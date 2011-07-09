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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GooglePlus.Cli
{
	class App
	{
		Api api;
		
		void SignIn ()
		{
			Console.Write ("email: ");
			var email = Console.ReadLine ();
			Console.Write ("password: ");
			var pwd = ReadPassword ();
			
			api = new Api ();
			api.SignIn (email, pwd);
		}
		
		void HelpCommand ()
		{
			Console.WriteLine ("Google+ command line interface by Frank Krueger (@praeclarum on Twitter)");
			Console.WriteLine ("Commands: ");
			foreach (var cmd in commands) {
				Console.WriteLine ("  " + cmd.Key);
			}			
		}
		
		void Repl ()
		{
			for (;;) {
				Console.Write ("G+> ");
				var cmdline = Console.ReadLine ();
				var parts = cmdline.Split (new char[] {' ','\t','\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 0) continue;
				var cmd = parts [0];
				var m = default(MethodInfo);
				if (commands.TryGetValue (cmd.ToLowerInvariant (), out m)) {
					m.Invoke (null, new object[] { parts.Skip (1).ToArray () });
					Console.WriteLine ();
				}
				else {
					Console.WriteLine ("unknown command: " + cmd);
				}
			}
		}
		
		static Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo> ();
		
		static App () {
			foreach (var m in typeof(App).GetMethods (BindingFlags.NonPublic | BindingFlags.Instance)) {
				if (m.Name.EndsWith ("Command")) {
					commands.Add (m.Name.Substring (0, m.Name.Length-7).ToLowerInvariant (), m);
				}
			}
		}
		
		public static void Main (string[] args)
		{
			var app = new App ();
			app.HelpCommand ();
			app.SignIn ();
			app.Repl ();
		}
		
		static string ReadPassword ()
		{
			var password = "";
			var info = Console.ReadKey (true);
			while (info.Key != ConsoleKey.Enter) {
				if (info.Key != ConsoleKey.Backspace) {
					password += info.KeyChar;
					info = Console.ReadKey (true);
				} 
				else if (info.Key == ConsoleKey.Backspace) {
					if (!string.IsNullOrEmpty (password)) {
						password = password.Substring
			            (0, password.Length - 1);
					}
					info = Console.ReadKey (true);
				}
			}
			return password;
		}
	}
}
