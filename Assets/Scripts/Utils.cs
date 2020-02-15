
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public static class Utils
{
  const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

  // This doesn't do what I was hoping for, but it is interesting:
  //  https://stackoverflow.com/questions/1028136/random-entry-from-dictionary
  //  It return a random ditionary item
  public static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
  {
    System.Random rand = new System.Random();
    List<TValue> values = Enumerable.ToList(dict.Values);
    int size = dict.Count;
    while (true)
    {
      yield return values[rand.Next(size)];
    }
  }

  // More complex version here:
  // https://stackoverflow.com/questions/4616685/how-to-generate-a-random-string-and-specify-the-length-you-want-or-better-gene
  public static string getRandomString(
      this System.Random rnd)
  {
    // ISet<string> usedRandomStrings = new HashSet<string>();
    char[] chars = new char[26];
    int setLength = AllowedChars.Length;

    int stringLength = 10;

    for (int i = 0; i < stringLength; ++i)
    {
      chars[i] = AllowedChars[rnd.Next(setLength)];
    }

    string randomString = new string(chars, 0, stringLength);


    return randomString;

    // if (usedRandomStrings.Add(randomString))
    // {
    //   yield return randomString;
    // }


  }
}
