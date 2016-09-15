using System;
using System.Text;
using Microsoft.VisualBasic;

namespace Vendare.Utils
{
	/// <summary>
	/// Summary description for RC4Alg. Pulled from http://aspnet.4guysfromrolla.com/code/rc4encrypt.cs.htm
	/// </summary>
	public class RC4Alg
	{
		protected int[] sbox = new int[256];
		protected int[] key = new int[256];

		private void init(string strPwd)
		{
			// Get the length of the password
			// Instead of Len(), we need to use the Length property
			// of the string
			int intLength = strPwd.Length;

			// Set up our for loop.  In C#, we need to change our syntax.

			// The first argument is the initializer.  Here we declare a
			// as an integer and set it equal to zero.

			// The second argument is expression that is used to test
			// for the loop termination.  Since our arrays have 256
			// elements and are always zero based, we need to loop as long
			// as a is less than or equal to 255.

			// The third argument is an iterator used to increment the
			// value of a by one each time through the loop.  Note that
			// we can use the ++ increment notation instead of a = a + 1
			for (int a = 0; a <= 255; a++)
			{
				// Since we don't have Mid()  in C#, we use the C#
				// equivalent of Mid(), String.Substring, to get a
				// single character from strPwd.  We declare a character
				// variable, ctmp, to hold this value.

				// A couple things to note.  First, the Mod keyword we
				// used in VB need to be replaced with the %
				// operator C# uses.  Next, since the return type of
				// String.Substring is a string, we need to convert it to
				// a char using String.ToCharArray() and specifying that
				// we want the first value in the array, [0].

				char ctmp = (strPwd.Substring((a % intLength),
					1).ToCharArray()[0]);

				// We now have our character and need to get the ASCII
				// code for it.  C# doesn't have the  VB Asc(), but that
				// doesn't mean we can't use it.  In the beginning of our
				// code, we imported the Microsoft.VisualBasic namespace.
				// This allows us to use many of the native VB functions
				// in C#
                
				// Note that we need to use [] instead of () for our
				// array members.
				key[a] = (int)Encoding.Default.GetBytes(new char[] {ctmp})[0];
				sbox[a] = a;
			}

			// Declare an integer x and initialize it to zero.
			int x = 0;

			// Again, create a for loop like the one above.  Note that we
			// need to use a different variable since we've already
			// declared a above.
			for (int b = 0; b <= 255; b++)
			{
				x = (x + sbox[b] + key[b]) % 256;
				int tempSwap = sbox[b];
				sbox[b] = sbox[x];
				sbox[x] = tempSwap;
			}
		}
		
		public string EnDeCrypt(string plaintext, string strPwd)
		{
			int i = 0;
			int j = 0;
			string cipher = "";

			// Set up a for loop.  Again, we use the Length property
			// of our String instead of the Len() function
			init(strPwd);

			for (int a = 1; a <= plaintext.Length; a++)
			{
				// Initialize an integer variable we will use in this loop
				int itmp = 0;


				// Like the RC4Initialize method, we need to use the %
				// in place of Mod
				i = (i + 1) % 256;
				j = (j + sbox[i]) % 256;
				itmp = sbox[i];
				sbox[i] = sbox[j];
				sbox[j] = itmp;

				int k = sbox[(sbox[i] + sbox[j]) % 256];

				// Again, since the return type of String.Substring is a
				// string, we need to convert it to a char using
				// String.ToCharArray() and specifying that we want the
				// first value, [0].

				char ctmp = plaintext.Substring(a - 1, 1).ToCharArray()
					[0];

				// Use Asc() from the Microsoft.VisualBasic namespace
				itmp = (int)Encoding.Default.GetBytes(new char[] {ctmp})[0];
				//itmp = Microsoft.VisualBasic.Strings.Asc(ctmp);


				// Here we need to use ^ operator that C# uses for Xor
				int cipherby = itmp ^ k;

				// Use Chr() from the Microsoft.VisualBasic namespace                
				cipher += Encoding.Default.GetString(new byte[] {(byte)cipherby});
				//cipher += Microsoft.VisualBasic.Strings.Chr(cipherby);
			}

			// Return the value of cipher as the return value of our
			// method
			return cipher;
		}

        public string EncDecryptFromBase64(string plaintext, string strPwd)
        {
            //decode url to conform to base64
            plaintext = plaintext.Replace('*', '+');
            plaintext = plaintext.Replace('-', '/');
            int pad = 4 - (plaintext.Replace(" ", "").Length % 4);
            for (int i = 0; i < pad && pad < 4; i++)
                plaintext += "=";
            // Antispyware = System.Text.Encoding.Default.GetString(HttpServerUtility.UrlTokenDecode(Antispyware));
            plaintext = System.Text.Encoding.Default.GetString(Convert.FromBase64String(plaintext));
            return EnDeCrypt(plaintext, strPwd);
        }

        public string EncDecryptToBase64(string plaintext, string strPwd)
        {
            plaintext = EnDeCrypt(plaintext, strPwd);
            plaintext = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(plaintext));
            //decode url to conform to base64
            plaintext = plaintext.Replace('+', '*');
            plaintext = plaintext.Replace('/', '-');
            plaintext = plaintext.Replace('=', ' ');
            plaintext = plaintext.Replace(" ", "");

            return plaintext;
        }
	}
}
