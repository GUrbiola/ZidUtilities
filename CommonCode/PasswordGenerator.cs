using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{
    /// <summary>
    /// Specifies the type of character component to be used in password generation.
    /// </summary>
    public enum PasswordComponentType
    {
        LowerCase,
        UpperCase,
        Digit,
        SpecialChar,
    }

    /// <summary>
    /// Represents a rule for a password component, specifying the type and quantity of characters.
    /// </summary>
    public class PasswordComponent
    {
        /// <summary>
        /// Gets or sets the type of password component (e.g., lower case, upper case, digit, special character).
        /// </summary>
        public PasswordComponentType CompType { get; set; }

        /// <summary>
        /// Gets or sets the quantity of characters of the specified type to include in the password.
        /// </summary>
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Generates passwords based on configurable rules and length.
    /// </summary>
    public class PasswordGenerator
    {
        /// <summary>
        /// Gets or sets the list of rules defining the types and quantities of characters to include in the password.
        /// </summary>
        public List<PasswordComponent> Rules { get; set; }

        /// <summary>
        /// Gets or sets the total length of the generated password.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGenerator"/> class with default settings.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="Rules"/> to an empty list and <see cref="Length"/> to 8 characters.
        /// </remarks>
        public PasswordGenerator()
        {
            this.Rules = new List<PasswordComponent>();
            this.Length = 8;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGenerator"/> class with a specified password length.
        /// </summary>
        /// <param name="length">
        /// Desired total length of the generated password. The minimum enforced length is 3 characters.
        /// </param>
        /// <remarks>
        /// Sets <see cref="Rules"/> to an empty list and <see cref="Length"/> to the maximum of 3 and the provided <paramref name="length"/>.
        /// </remarks>
        public PasswordGenerator(int length)
        {
            this.Rules = new List<PasswordComponent>();
            this.Length = Math.Max(3, length);
        }

        /// <summary>
        /// Generates a password string based on the configured <see cref="Rules"/> and total <see cref="Length"/>.
        /// </summary>
        /// <param name="lucky">
        /// Optional <see cref="System.Random"/> instance to drive randomness. If null, a new <see cref="System.Random"/> is created.
        /// </param>
        /// <returns>
        /// A password string that:
        /// - Includes the quantities of character types defined in <see cref="Rules"/>.
        /// - Is padded with random upper or lower case characters until it reaches <see cref="Length"/>.
        /// - Is shuffled before returning to avoid predictable ordering.
        /// </returns>
        /// <remarks>
        /// Character sets used:
        /// - Uppercase A-Z
        /// - Lowercase a-z
        /// - Digits 0-9
        /// - Readable special characters: @ ? _ ! # % $ & + -
        /// If total quantities in <see cref="Rules"/> are less than <see cref="Length"/>, the method fills the remainder
        /// with random letters (upper/lower). The final result is shuffled via <c>newPass.Shuffle&lt;string&gt;()</c>.
        /// </remarks>
        public string GetPassword(Random lucky = null)
        {
            List<string> newPass = new List<string>();
            List<string> upperCase = new List<string>()
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };
            List<string> lowerCase = new List<string>()
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
                "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
            };
            List<string> digits = new List<string>()
            {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
            };
            List<string> readableSpecial = new List<string>()
            {
                "@", "?", "_", "!", "#", "%", "$", "&", "+", "-"
            };

            Random random = lucky == null ? new Random() : lucky;
            foreach (PasswordComponent rule in this.Rules)
            {
                switch (rule.CompType)
                {
                    case PasswordComponentType.LowerCase:
                        for (int index = 0; index < rule.Quantity; ++index)
                            newPass.Add(lowerCase[random.Next(lowerCase.Count)]);
                        continue;
                    case PasswordComponentType.UpperCase:
                        for (int index = 0; index < rule.Quantity; ++index)
                            newPass.Add(upperCase[random.Next(upperCase.Count)]);
                        continue;
                    case PasswordComponentType.Digit:
                        for (int index = 0; index < rule.Quantity; ++index)
                            newPass.Add(digits[random.Next(digits.Count)]);
                        continue;
                    case PasswordComponentType.SpecialChar:
                        for (int index = 0; index < rule.Quantity; ++index)
                            newPass.Add(readableSpecial[random.Next(readableSpecial.Count)]);
                        continue;
                    default:
                        continue;
                }
            }

            // If we are still short, add random chars to fill the gap
            while (newPass.Count < this.Length)
                newPass.Add(random.Next(2) > 0 ? lowerCase[random.Next(lowerCase.Count)] : upperCase[random.Next(upperCase.Count)]);

            newPass.Shuffle<string>();
            return string.Join("", (IEnumerable<string>)newPass);
        }
    }
}
