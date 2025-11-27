using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{
    /// <summary>
    /// Check - Validation and verification support.
    /// Provides static methods for validating ranges, null checks, emptiness, and type checks.
    /// </summary>
    public static class Check
    {
        #region Range Methods
        #region RangeMaxExclusive
        /// <summary>
        /// Validates that a double value is less than the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not less than max.</exception>
        public static void RangeMaxExclusive(double val, string paramName, double max)
        {
            if (!(val < max))
            {
                throw new ArgumentException("The value, " + paramName + ", must be less than " + max + "; it's value is " + val + ".");
            }
        }

        /// <summary>
        /// Validates that a DateTime value is less than the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not less than max.</exception>
        public static void RangeMaxExclusive(DateTime val, string paramName, DateTime max)
        {
            if (!(val < max))
            {
                throw new ArgumentException(string.Format("The value, {0}, must be less than {1}; it's value is {2}.", paramName, max.ToShortDateString(), val.ToShortDateString()));
            }
        }

        /// <summary>
        /// Validates that an int value is less than the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not less than max.</exception>
        public static void RangeMaxExclusive(int val, string paramName, int max)
        {
            RangeMaxExclusive((double)val, paramName, (double)max);
        }

        /// <summary>
        /// Validates that a decimal value is less than the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not less than max.</exception>
        public static void RangeMaxExclusive(decimal val, string paramName, decimal max)
        {
            RangeMaxExclusive((double)val, paramName, (double)max);
        }

        /// <summary>
        /// Validates that a float value is less than the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not less than max.</exception>
        public static void RangeMaxExclusive(float val, string paramName, float max)
        {
            RangeMaxExclusive((double)val, paramName, (double)max);
        }
        #endregion

        #region RangeMinExclusive
        /// <summary>
        /// Validates that a double value is greater than the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not greater than min.</exception>
        public static void RangeMinExclusive(double val, string paramName, double min)
        {
            if (!(val > min))
            {
                throw new ArgumentException("The value, " + paramName + ", must be greater than " + min + "; it's value is " + val + ".");
            }
        }

        /// <summary>
        /// Validates that a DateTime value is greater than the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not greater than min.</exception>
        public static void RangeMinExclusive(DateTime val, string paramName, DateTime min)
        {
            if (!(val > min))
            {
                throw new ArgumentException(string.Format("The value, {0}, must be greater than {1}; it's value is {2}.", paramName, min.ToShortDateString(), val.ToShortDateString()));
            }
        }

        /// <summary>
        /// Validates that an int value is greater than the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not greater than min.</exception>
        public static void RangeMinExclusive(int val, string paramName, int min)
        {
            RangeMinExclusive((double)val, paramName, (double)min);
        }

        /// <summary>
        /// Validates that a decimal value is greater than the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not greater than min.</exception>
        public static void RangeMinExclusive(decimal val, string paramName, decimal min)
        {
            RangeMinExclusive((double)val, paramName, (double)min);
        }

        /// <summary>
        /// Validates that a float value is greater than the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is not greater than min.</exception>
        public static void RangeMinExclusive(float val, string paramName, float min)
        {
            RangeMinExclusive((double)val, paramName, (double)min);
        }
        #endregion

        #region RangeMaxInclusive
        /// <summary>
        /// Validates that a double value is less than or equal to the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is greater than max.</exception>
        public static void RangeMaxInclusive(double val, string paramName, double max)
        {
            if (!(val <= max))
            {
                throw new ArgumentException("The value, " + paramName + ", must be less than or equal to " + max + "; it's value is " + val + ".");
            }
        }

        /// <summary>
        /// Validates that a DateTime value is less than or equal to the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is greater than max.</exception>
        public static void RangeMaxInclusive(DateTime val, string paramName, DateTime max)
        {
            if (!(val <= max))
            {
                throw new ArgumentException(string.Format("The value, {0}, must be less than or equal to {1}; it's value is {2}.", paramName, max.ToShortDateString(), val.ToShortDateString()));
            }
        }

        /// <summary>
        /// Validates that an int value is less than or equal to the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is greater than max.</exception>
        public static void RangeMaxInclusive(int val, string paramName, int max)
        {
            RangeMaxInclusive((double)val, paramName, (double)max);
        }

        /// <summary>
        /// Validates that a decimal value is less than or equal to the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is greater than max.</exception>
        public static void RangeMaxInclusive(decimal val, string paramName, decimal max)
        {
            RangeMaxInclusive((double)val, paramName, (double)max);
        }

        /// <summary>
        /// Validates that a float value is less than or equal to the specified maximum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="max">Maximum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is greater than max.</exception>
        public static void RangeMaxInclusive(float val, string paramName, float max)
        {
            RangeMaxInclusive((double)val, paramName, (double)max);
        }
        #endregion

        #region RangeMinInclusive
        /// <summary>
        /// Validates that a double value is greater than or equal to the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is less than min.</exception>
        public static void RangeMinInclusive(double val, string paramName, double min)
        {
            if (!(val >= min))
            {
                throw new ArgumentException("The value, " + paramName + ", must be greater or equal to " + min + "; it's value is " + val + ".");
            }
        }

        /// <summary>
        /// Validates that a DateTime value is greater than or equal to the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is less than min.</exception>
        public static void RangeMinInclusive(DateTime val, string paramName, DateTime min)
        {
            if (!(val >= min))
            {
                throw new ArgumentException(string.Format("The value, {0}, must be greater than or equal to {1}; it's value is {2}.", paramName, min.ToShortDateString(), val.ToShortDateString()));
            }
        }

        /// <summary>
        /// Validates that an int value is greater than or equal to the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is less than min.</exception>
        public static void RangeMinInclusive(int val, string paramName, int min)
        {
            RangeMinInclusive((double)val, paramName, (double)min);
        }

        /// <summary>
        /// Validates that a decimal value is greater than or equal to the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is less than min.</exception>
        public static void RangeMinInclusive(decimal val, string paramName, decimal min)
        {
            RangeMinInclusive((double)val, paramName, (double)min);
        }

        /// <summary>
        /// Validates that a float value is greater than or equal to the specified minimum.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <exception cref="ArgumentException">Thrown if val is less than min.</exception>
        public static void RangeMinInclusive(float val, string paramName, float min)
        {
            RangeMinInclusive((double)val, paramName, (double)min);
        }
        #endregion

        #region RangeInclusive
        /// <summary>
        /// Validates that a double value is within the specified inclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (inclusive).</param>
        public static void RangeInclusive(double val, string paramName, double min, double max)
        {
            RangeMinInclusive(val, paramName, min);
            RangeMaxInclusive(val, paramName, max);
        }

        /// <summary>
        /// Validates that a DateTime value is within the specified inclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (inclusive).</param>
        public static void RangeInclusive(DateTime val, string paramName, DateTime min, DateTime max)
        {
            RangeMinInclusive(val, paramName, min);
            RangeMaxInclusive(val, paramName, max);
        }

        /// <summary>
        /// Validates that an int value is within the specified inclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (inclusive).</param>
        public static void RangeInclusive(int val, string paramName, int min, int max)
        {
            RangeInclusive((double)val, paramName, (double)min, (double)max);
        }

        /// <summary>
        /// Validates that a float value is within the specified inclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (inclusive).</param>
        public static void RangeInclusive(float val, string paramName, float min, float max)
        {
            RangeInclusive((double)val, paramName, (double)min, (double)max);
        }

        /// <summary>
        /// Validates that a decimal value is within the specified inclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (inclusive).</param>
        public static void RangeInclusive(decimal val, string paramName, decimal min, decimal max)
        {
            RangeInclusive((double)val, paramName, (double)min, (double)max);
        }
        #endregion

        #region RangeExclusive
        /// <summary>
        /// Validates that a double value is within the specified exclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public static void RangeExclusive(double val, string paramName, double min, double max)
        {
            RangeMinExclusive(val, paramName, min);
            RangeMaxExclusive(val, paramName, max);
        }

        /// <summary>
        /// Validates that a DateTime value is within the specified exclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public static void RangeExclusive(DateTime val, string paramName, DateTime min, DateTime max)
        {
            RangeMinExclusive(val, paramName, min);
            RangeMaxExclusive(val, paramName, max);
        }

        /// <summary>
        /// Validates that an int value is within the specified exclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public static void RangeExclusive(int val, string paramName, int min, int max)
        {
            RangeExclusive((double)val, paramName, (double)min, (double)max);
        }

        /// <summary>
        /// Validates that a decimal value is within the specified exclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public static void RangeExclusive(decimal val, string paramName, decimal min, decimal max)
        {
            RangeExclusive((double)val, paramName, (double)min, (double)max);
        }

        /// <summary>
        /// Validates that a float value is within the specified exclusive range.
        /// </summary>
        /// <param name="val">Value to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="min">Minimum value (exclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        public static void RangeExclusive(float val, string paramName, float min, float max)
        {
            RangeExclusive((double)val, paramName, (double)min, (double)max);
        }
        #endregion
        #endregion

        #region NotEmpty
        /// <summary>
        /// Validates that a byte array is not null or empty.
        /// </summary>
        /// <param name="array">Byte array to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <exception cref="ArgumentException">Thrown if array is null or empty.</exception>
        public static void NotEmpty(byte[] array, string paramName)
        {
            if (array == null || array.Length == 0)
            {
                throw new ArgumentException("The byte array, " + paramName + ", can't be empty.", paramName);
            }
        }

        /// <summary>
        /// Validates that a Guid is not empty.
        /// </summary>
        /// <param name="guid">Guid to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <exception cref="ArgumentException">Thrown if guid is Guid.Empty.</exception>
        public static void NotEmpty(Guid guid, string paramName)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("The guid, " + paramName + ", can't have an empty value.", paramName);
            }
        }

        /// <summary>
        /// Validates that a nullable integer has a value.
        /// </summary>
        /// <param name="x">Nullable integer to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <exception cref="ArgumentException">Thrown if x is null.</exception>
        public static void NotEmpty(int? x, string paramName)
        {
            if (!x.HasValue)
            {
                throw new ArgumentException("The int, " + paramName + ", can't have an empty value.", paramName);
            }
        }

        /// <summary>
        /// Validates that a string is not null or empty. Uses "[unnamed]" as the parameter name.
        /// </summary>
        /// <param name="str">String to test.</param>
        /// <exception cref="ArgumentException">Thrown if str is null or empty.</exception>
        public static void NotEmpty(string str)
        {
            NotEmpty(str, "[unnamed]");
        }

        /// <summary>
        /// Validates that a string is not null or empty.
        /// </summary>
        /// <param name="str">String to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <exception cref="ArgumentException">Thrown if str is null or empty.</exception>
        public static void NotEmpty(string str, string paramName)
        {
            NotEmpty(str, paramName, null);
        }

        /// <summary>
        /// Validates that a string is not null or empty.
        /// </summary>
        /// <param name="str">String to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="message">Custom message for exception.</param>
        /// <exception cref="ArgumentException">Thrown if str is null or empty.</exception>
        public static void NotEmpty(string str, string paramName, string message)
        {
            if (str == null || str.Trim().Length <= 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Validates that a collection is not null or empty.
        /// </summary>
        /// <param name="col">Collection to test.</param>
        /// <param name="paramName">Parameter name for error reporting.</param>
        /// <param name="message">Custom message for exception.</param>
        /// <exception cref="ArgumentException">Thrown if collection is null or empty.</exception>
        public static void NotEmpty(ICollection col, string paramName, string message)
        {
            if (col == null || col.Count == 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }
        #endregion

        #region NotNull
        /// <summary>
        /// Validates that an object is not null.
        /// </summary>
        /// <param name="obj">Object to test.</param>
        /// <param name="objectName">Name of object for error reporting.</param>
        /// <exception cref="NullReferenceException">Thrown if obj is null.</exception>
        public static void NotNull(object obj, string objectName)
        {
            NotNull(obj, objectName, null);
        }

        /// <summary>
        /// Validates that an object is not null.
        /// </summary>
        /// <param name="obj">Object to test.</param>
        /// <param name="objectName">Name of object for error reporting.</param>
        /// <param name="message">Custom message for exception.</param>
        /// <exception cref="NullReferenceException">Thrown if obj is null.</exception>
        public static void NotNull(object obj, string objectName, string message)
        {
            if (obj == null)
            {
                throw new NullReferenceException(string.Format("The object, {0}, can not be null. {1}", objectName, message));
            }
        }
        #endregion

        /// <summary>
        /// Validates that a boolean condition is false.
        /// </summary>
        /// <param name="condition">Boolean value to test.</param>
        /// <param name="conditionName">Name of the condition for error reporting.</param>
        /// <exception cref="ApplicationException">Thrown if condition is true.</exception>
        public static void NotTrue(bool condition, string conditionName)
        {
            True(!condition, conditionName);
        }

        /// <summary>
        /// Validates that a boolean condition is false.
        /// </summary>
        /// <param name="condition">Boolean value to test.</param>
        /// <param name="conditionName">Name of the condition for error reporting.</param>
        /// <param name="message">Custom message for exception.</param>
        /// <exception cref="ApplicationException">Thrown if condition is true.</exception>
        public static void NotTrue(bool condition, string conditionName, string message)
        {
            True(!condition, conditionName, message);
        }

        /// <summary>
        /// Validates that a boolean condition is true.
        /// </summary>
        /// <param name="condition">Boolean value to test.</param>
        /// <param name="conditionName">Name of the condition for error reporting.</param>
        /// <exception cref="ApplicationException">Thrown if condition is false.</exception>
        public static void True(bool condition, string conditionName)
        {
            True(condition, conditionName, null);
        }

        /// <summary>
        /// Validates that a boolean condition is true.
        /// </summary>
        /// <param name="condition">Boolean value to test.</param>
        /// <param name="conditionName">Name of the condition for error reporting.</param>
        /// <param name="message">Custom message for exception.</param>
        /// <exception cref="ApplicationException">Thrown if condition is false.</exception>
        public static void True(bool condition, string conditionName, string message)
        {
            if (!condition)
            {
                throw new ApplicationException(string.Format("The condition, {0}, is not true. {1}", conditionName, message));
            }
        }

        /// <summary>
        /// Validates that an object is of the specified type.
        /// </summary>
        /// <param name="obj">Object to test.</param>
        /// <param name="type">Type to compare.</param>
        /// <exception cref="ApplicationException">Thrown if object is not of the specified type.</exception>
        public static void TypeOf(object obj, Type type)
        {
            TypeOf(obj, type, null);
        }

        /// <summary>
        /// Validates that an object is of the specified type.
        /// </summary>
        /// <param name="obj">Object to test.</param>
        /// <param name="type">Type to compare.</param>
        /// <param name="message">Custom message for exception.</param>
        /// <exception cref="ApplicationException">Thrown if object is not of the specified type.</exception>
        public static void TypeOf(object obj, Type type, string message)
        {
            if (obj.GetType() != type)
            {
                throw new ApplicationException(string.Format("The object, {0}, is not of type {1}. {2}", obj.ToString(), type.FullName, message));
            }
        }
    }
}
