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
		
		public void SignIn (string email, string password)
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
			
			ScrapeHomeData (html);
			
			Console.WriteLine ("OK");
		}
		
		public void Refresh ()
		{
		}
		
		class HomeData
		{
			public readonly IDictionary<string, object> Dictionary;
			
			public HomeData (IDictionary<string, object> dictionary) {
				Dictionary = dictionary;
			}
		}
		
		class DataConverter : JavaScriptConverter
		{
			public override object Deserialize (IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
			{
				if (type == typeof (HomeData)) {
					return new HomeData (dictionary);
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
		
		void ScrapeHomeData (string html)
		{
			var i = html.IndexOf ("var OZ_initData");
			if (i < 0) return;
			i = html.IndexOf ('{', i);
			var e = html.IndexOf ("</script>", i);
			if (e < 0) return;
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
			
			System.Console.WriteLine (data);
		}
		
		string PostForm (string url, IDictionary<string, string> form)
		{
			var formEncoded = string.Join ("&", from i in form 
				select Uri.EscapeDataString (i.Key) + "=" + Uri.EscapeDataString (i.Value));
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
}

