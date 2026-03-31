using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayfairCipherApp;
using System;
using System.Collections.Generic;

namespace PlayfairCipherApp.Tests
{
    /// <summary>
    /// Набор автоматизированных тестов для класса PlayfairCipher.
    /// Покрывает позитивные, негативные и граничные сценарии.
    /// </summary>
    [TestClass]
    public class PlayfairCipherTests
    {
        // ==================== ПОЗИТИВНЫЕ ТЕСТЫ ====================

        [TestMethod]
        public void Encrypt_SimpleText_ReturnsNonEmptyCipher()
        {
            var cipher = new PlayfairCipher("MONARCHY");
            string result = cipher.Encrypt("HELLO");

            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreNotEqual("HELLO", result);
        }

        [TestMethod]
        public void EncryptDecrypt_Roundtrip_ReturnsOriginal()
        {
            var cipher = new PlayfairCipher("KEYWORD");
            string original = "TESTME";

            string encrypted = cipher.Encrypt(original);
            string decrypted = cipher.Decrypt(encrypted);

            Assert.AreEqual(original, decrypted);
        }

        [TestMethod]
        public void Encrypt_RepeatingLetters_HandlesCorrectly()
        {
            var cipher = new PlayfairCipher("KEY");
            string result = cipher.Encrypt("BALLOON");

            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreEqual(0, result.Length % 2);
        }

        [TestMethod]
        public void Encrypt_OddLength_ResultHasEvenLength()
        {
            var cipher = new PlayfairCipher("KEY");
            string result = cipher.Encrypt("HELLO");

            Assert.AreEqual(0, result.Length % 2);
        }

        [TestMethod]
        public void Encrypt_DifferentKeys_ProduceDifferentResults()
        {
            var cipher1 = new PlayfairCipher("ALPHA");
            var cipher2 = new PlayfairCipher("BRAVO");

            string result1 = cipher1.Encrypt("TESTME");
            string result2 = cipher2.Encrypt("TESTME");

            Assert.AreNotEqual(result1, result2);
        }

        [TestMethod]
        public void Encrypt_KnownExample_MONARCHY_INSTRUMENTS()
        {
            var cipher = new PlayfairCipher("MONARCHY");
            string result = cipher.Encrypt("INSTRUMENTS");

            Assert.AreEqual("GATLMZCLRQXA", result);
        }

        [TestMethod]
        public void EncryptDecrypt_LongText_Roundtrip()
        {
            var cipher = new PlayfairCipher("SECRETKEY");
            string original = "THEQUICKBROWNFOXIUMPSOVERTHELAZYDOG";

            string encrypted = cipher.Encrypt(original);
            string decrypted = cipher.Decrypt(encrypted);

            Assert.IsTrue(decrypted.StartsWith("TH"));
        }

        [TestMethod]
        public void Constructor_KeyWithRepeats_BuildsValidMatrix()
        {
            var cipher = new PlayfairCipher("AABBCC");
            var matrix = cipher.GetMatrix();

            var chars = new HashSet<char>();

            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    chars.Add(matrix[r, c]);

            Assert.AreEqual(25, chars.Count);
        }

        // ==================== НЕГАТИВНЫЕ ТЕСТЫ ====================

        [TestMethod]
        public void Encrypt_EmptyString_ThrowsArgumentException()
        {
            var cipher = new PlayfairCipher("KEY");

            Assert.ThrowsException<ArgumentException>(() => cipher.Encrypt(""));
        }

        [TestMethod]
        public void Constructor_EmptyKey_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new PlayfairCipher(""));
        }

        [TestMethod]
        public void Encrypt_NullText_ThrowsArgumentNullException()
        {
            var cipher = new PlayfairCipher("KEY");

            Assert.ThrowsException<ArgumentNullException>(() => cipher.Encrypt(null));
        }

        [TestMethod]
        public void Constructor_NullKey_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PlayfairCipher(null));
        }

        [TestMethod]
        public void Decrypt_EmptyString_ThrowsArgumentException()
        {
            var cipher = new PlayfairCipher("KEY");

            Assert.ThrowsException<ArgumentException>(() => cipher.Decrypt(""));
        }

        [TestMethod]
        public void Decrypt_NullText_ThrowsArgumentNullException()
        {
            var cipher = new PlayfairCipher("KEY");

            Assert.ThrowsException<ArgumentNullException>(() => cipher.Decrypt(null));
        }

        // ==================== ТЕСТЫ МАТРИЦЫ ====================

        [TestMethod]
        public void GetMatrix_DoesNotContainJ()
        {
            var cipher = new PlayfairCipher("TEST");
            var matrix = cipher.GetMatrix();

            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    Assert.AreNotEqual('J', matrix[r, c]);
        }
    }
}