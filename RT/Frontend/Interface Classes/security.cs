using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace RT {
	static class security {

		static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("89sdf78s9dfsdf(*F(SD9d8fSD(F*&*S(D#@$^");
		
		public static string EncryptString(string input) {
			byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
				System.Text.Encoding.Unicode.GetBytes(input),
				entropy,
				System.Security.Cryptography.DataProtectionScope.CurrentUser);
		
			return Convert.ToBase64String(encryptedData);
		}

		public static string DecryptString(string encryptedData) {
			try {
				byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
					Convert.FromBase64String(encryptedData),
					entropy,
					System.Security.Cryptography.DataProtectionScope.CurrentUser);

				return System.Text.Encoding.Unicode.GetString(decryptedData);
			} catch {
				return String.Empty;
			}
		}
	}
}
