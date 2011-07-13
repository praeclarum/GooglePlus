//
// Copyright (c) 2011 Frank A. Krueger
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
		
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Collections;

namespace GooglePlus
{
	public class Api
	{
		CookieContainer cookies = new CookieContainer ();
		string gpcaz = null;
		string galx = null;
		
		public Api ()
		{
		}
		
		public HomeData SignIn (string email, string password)
		{
			Console.WriteLine ("Logging in as {0}...", email);
			
			Get ("http://plus.google.com");
			
			Get ("https://www.google.com/accounts/ServiceLogin?service=oz&continue=https://plus.google.com/?gpcaz%3D"+gpcaz+"&ltmpl=es2st&hideNewAccountLink=1&hl=en-US");
			
			var html = PostForm ("https://www.google.com/accounts/ServiceLoginAuth", new Dictionary<string,string> () {				
				{"ltmpl", "es2st"},
			    {"pstMsg", "1"},
			    {"dnConn", "https://accounts.youtube.com"},
			    {"continue", "https://plus.google.com/?gpcaz=" + gpcaz},
			    {"service", "oz"},
				//{"dsh", "NNNNNNNNNNNNNNNNNNN"},
			    //{"ltmpl", "es2st"},
			    {"hl", "en-US"},
			    //{"ltmpl", "es2st"},
			    {"timeStmp", ""},
			    {"secTok", ""},
			    {"GALX", galx},
			    {"Email", email},
			    {"Passwd", password},
			    {"PersistentCookie", "yes"},
			    {"rmShown", "1"},
			    {"signIn", "Sign in"},
				{"asts", ""},
			});
			
			return ScrapeHomeData (html);
		}
		
		HomeData ScrapeHomeData (string html)
		{
			var i = html.IndexOf ("var OZ_initData");
			if (i < 0) return null;
			i = html.IndexOf ('{', i);
			var e = html.IndexOf ("</script>", i);
			if (e < 0) return null;
			while (e > i && html[e] != '}') {
				e--;
			}
			e++;
			
			var script = html
				.Substring (i, e - i)
				.Replace ("[,", "[\"\",")
				.Replace (",,", ",\"\",")
				.Replace (",,", ",\"\",")
				;
			
			var jss = new JavaScriptSerializer();
			jss.RegisterConverters (new JavaScriptConverter[] {
				new DataConverter (),
			});
			var data = jss.Deserialize<HomeData> (script);
			
			return data;
		}
		
		string PostForm (string url, IDictionary<string, string> form)
		{
			var formEncoded = string.Join ("&", (from i in form 
				select Uri.EscapeDataString (i.Key) + "=" + Uri.EscapeDataString (i.Value)).ToArray ());
			var content = Encoding.UTF8.GetBytes (formEncoded);
			var req = CreateRequest (url);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = content.Length;
			using (var s = req.GetRequestStream ()) {
				s.Write (content, 0, content.Length);
			}
			return ReadResponseText (req);
		}
		
		string Get (string url)
		{
			return ReadResponseText (CreateRequest (url));
		}

		HttpWebRequest CreateRequest (string url)
		{
			var req = (HttpWebRequest)WebRequest.Create (url);
			//req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6) Mono/2.10 (HTML5, like Gecko) GooglePlus/1.0";
			// G+ is picky about User-Agents. So anti-web.
			req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:5.0) Gecko/20100101 Firefox/5.0";
			req.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
			req.Headers.Add ("Accept-Charset", "utf-8");
			req.AllowAutoRedirect = true;
			req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			req.CookieContainer = cookies;
			return req;
		}

		string ReadResponseText (HttpWebRequest req)
		{
			using (var resp = (HttpWebResponse)req.GetResponse ()) {
				using (var s = resp.GetResponseStream ()) {
					
					if (gpcaz == null) {
						var q = resp.ResponseUri.Query;
						var i = q.IndexOf ("gpcaz=");
						if (i >= 0) {
							var e = q.IndexOf ('&', i); if (e < 0) e = q.Length;
							gpcaz = q.Substring (i + 6, e - (i+6));
						}
					}
					if (galx == null) {
						foreach (Cookie c in resp.Cookies) {
							if (c.Name == "GALX") {
								galx = c.Value;
							}
						}
					}
					
					using (var r = new StreamReader(s, Encoding.UTF8)) {
						return r.ReadToEnd ();
					}
				}
			}
		}
	}
	
	class DataConverter : JavaScriptConverter
	{
		public override object Deserialize (IDictionary<string, object> d, Type type, JavaScriptSerializer serializer)
		{
			if (type == typeof (HomeData)) {
				return new HomeData (d);
			}
			else throw new ArgumentException ("type");
		}
		public override IDictionary<string, object> Serialize (object obj, JavaScriptSerializer serializer)
		{
			throw new NotImplementedException ();
		}
		public override IEnumerable<Type> SupportedTypes {
			get {
				yield return typeof(HomeData);
			}
		}
	}
	
	public class Data
	{
		public readonly IList DataList;
		public Data (IList items) {
			DataList = items;
		}
		protected List<T> ListAt<T> (int i, Func<IList, T> ctor)
		{
			var list = new List<T> ();
			var dlist = (i < DataList.Count) ? DataList [i] as IList : null;
			if (dlist != null && dlist.Count > 0) {
				list.AddRange (from o in dlist.Cast<object> () 
					let l = o as IList 
					where l != null
					select ctor (l));
			}
			return list;
		}
		protected T ObjectAt<T> (int i, Func<IList, T> ctor) where T : class
		{
			if (i >= DataList.Count) return null;
			var dlist = DataList [i] as IList;
			if (dlist == null) return null;
			return ctor (dlist);
		}
		protected string StringAt (int i)
		{
			if (i >= DataList.Count) return "";
			var o = DataList [i];
			if (o == null) return "";
			return o.ToString ();
		}
		protected int ReadInt (int i)
		{
			if (i >= DataList.Count) return 0;
			var o = DataList [i];
			if (o == null) return 0;
			if (o is int) return (int)o;
			var s = o.ToString ();
			if (s.Length == 0) return 0;
			if (!char.IsDigit (s [0]) && s [0] != '-') return 0;
			var iv = 0;
			int.TryParse (s, out iv);
			return iv;
		}
		protected Uri UrlAt (int i)
		{
			var s = StringAt (i);
			if (s.Length == 0) return null;
			try {
				return new Uri (s);
			}
			catch (Exception) {
				return null;
			}
		}
	}
	
	public class HomeData : Data
	{
		public StreamItemsData StreamItems;
		public CirclesData Circles;
		
		public HomeData (IDictionary<string, object> d)
			: base (new ArrayList ()) 
		{
			StreamItems = new StreamItemsData ((IList)d ["4"]);
			Circles = new CirclesData ((IList)d ["12"]);
		}
	}
	public class Media : Data
	{
		public Uri Url;
        public string ContentType;
        public string ShortContentType;

		public Media (IList d)
			: base (d) 
		{
			Url = UrlAt (1);
			ContentType = StringAt (3);
			ShortContentType = StringAt (4);
		}
	}
	public class MetadataEntry : Data
	{
		public string Key;
		public string Value;

		public MetadataEntry (IList d)
			: base (d) 
		{
			Value = StringAt (1);
			Key = StringAt (2);
		}
	}
	public class Thumbnail : Data
	{
		public Uri ResourceUrl;
        public int Height;
        public int Width;

		public Thumbnail (IList d)
			: base (d) 
		{
			ResourceUrl = UrlAt (1);
			Height = ReadInt (2);
			Width = ReadInt (3);
		}
	}
	public class Attachment : Data
	{
        public Thumbnail Preview;
        public Uri SomeImageUrl;
        public Media Media;
        public List<Thumbnail> Thumbnails;
        public List<MetadataEntry> Metadata;
				
		public Attachment (IList d)
			: base (d) 
		{
			Preview = ObjectAt (5, x => new Thumbnail (x));
			SomeImageUrl = UrlAt (21);
			Media = ObjectAt (24, x => new Media (x));
			Thumbnails = ListAt (41, x => new Thumbnail (x));
			Metadata = ListAt (47, x => new MetadataEntry (x));
		}
	}
	
	public class Voter : Data
	{
        public string FullName;
        public string UserId;
        public Uri UserUrl;
        public Uri UserPhotoUrl;
		
		public Voter (IList d)
			: base (d) 
		{
			FullName = StringAt (0);
			UserId = StringAt (1);
			UserUrl = UrlAt (2);
			UserPhotoUrl = UrlAt (3);
		}
	}
	
	public class Voting : Data
	{
		public string UrlId;
        public int VoteType; // 4 = post, 5 = comment
        public int TotalVotes;
        public List<Voter> KnownVoters;

		public Voting (IList d)
			: base (d) 
		{
			UrlId = StringAt (0);
			VoteType = ReadInt (1);
			TotalVotes = ReadInt (16);
			KnownVoters = ListAt (17, x => new Voter (x));
		}
	}
	
	public class Comment : Data
	{
		public string UserFullName;
        public string FullTextHtml;
        public string ItemCommentsId;
        public string CommentInCommentsUrlId;
        public string FullTextRaw;
        public string UserId;
        public string CommentUrlId;
        public string UserUrl;
        public Voting Voting;
        public Uri UserPhotoUrl;

		public Comment (IList d)
			: base (d) 
		{
			UserFullName = StringAt (1);
			FullTextHtml = StringAt (2);
			ItemCommentsId = StringAt (3);
			CommentInCommentsUrlId = StringAt (4);
			FullTextRaw = StringAt (5);
			UserId = StringAt (6);
			CommentUrlId = StringAt (7);
			UserUrl = StringAt (10);
			Voting = ObjectAt (15, x => new Voting (x));
			UserPhotoUrl = UrlAt (16);
		}
	}
	
	public class Item : Data
	{
		public string ItemType;
        public string UserFullName;
        public string FullTextHtml;
        public string ItemId;
        public Uri FaviconUrl;
		public List<Comment> VisibleComments;
		public string ItemUrlId;
		public List<Attachment> Attachments;
		public Voting Voting;		
		public string FullTextIntermediate;
        public string UserId;
        public Uri PhotoUrl;
        public string FullTextRaw;
        public string PostUrl;
        public string SomeId;
        public string UserUrl;
        //public string RelatedUsers: UserRef[]
        public string ItemCommentsId;
        public string PreviewHtml;

		public Item (IList d)
			: base (d) 
		{
			ItemType = StringAt (2);
			UserFullName = StringAt (3);
			FullTextHtml = StringAt (4);
			ItemId = StringAt (5);
			FaviconUrl = UrlAt (6);
			VisibleComments = ListAt (7, x => new Comment (x));
			ItemUrlId = StringAt (8);
			Attachments = ListAt (11, x => new Attachment (x));
			Voting = ObjectAt (73, x => new Voting (x));			
			FullTextIntermediate = StringAt (14);
			UserId = StringAt (16);
			PhotoUrl = UrlAt (18);
			FullTextRaw = StringAt (20);
			PostUrl = StringAt (21);
			SomeId = StringAt (22);
			UserUrl = StringAt (24);
			ItemCommentsId = StringAt (30);
			PreviewHtml = StringAt (33);
		}
	}
	
	public class StreamItemsData : Data
	{
		public readonly List<Item> Items;
		
		public StreamItemsData (IList d)
			: base (d) 
		{
			Items = ListAt (0, x => new Item (x));
		}
	}
	
	public class Circle : Data
	{
		public string CircleId;
        public string Name;
		public string Description;
		public string UrlId;
		public int Order;

		public Circle (IList d)
			: base (d) 
		{
			CircleId = ((IList)(d [0]))[0].ToString ();
			var d1 = (IList)d [1];
			Name = d1 [0].ToString ();
			Description = d1 [2].ToString ();
			UrlId = d1 [12].ToString ();
			int.TryParse (d1 [13].ToString (), out Order);
		}
	}
	
	public class CirclesData : Data
	{
		public readonly List<Circle> Circles;
		
		public CirclesData (IList d)
			: base (d) 
		{
			Circles = ListAt (0, x => new Circle (x));
		}
	}
}

