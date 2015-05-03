namespace Wacton.Tovarisch.Lexicon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Wacton.Tovarisch.Properties;
    using Wacton.Tovarisch.Randomness;

    public class WordProvider
    {
        private const int FirstDataIndex = 29;
        private const string NewLine = "\n";
        private const string AnyNonAlphabeticCharactersPattern = @"[\W\d_]";

        private readonly Dictionary<WordClass, List<string>> wordsByClass = new Dictionary<WordClass, List<string>>();

        public WordProvider()
        {
            this.wordsByClass.Add(WordClass.Adjective, ParseResourceWordData(Resources.Adjectives));
            this.wordsByClass.Add(WordClass.Adverb, ParseResourceWordData(Resources.Adverbs));
            this.wordsByClass.Add(WordClass.Noun, ParseResourceWordData(Resources.Nouns));
            this.wordsByClass.Add(WordClass.Verb, ParseResourceWordData(Resources.Verbs));
        }

        public string GetRandomWord(WordClass wordClass)
        {
            // random integer upper bound is exclusive: 0 <= n < wordCount - index will always be in range
            var wordList = this.wordsByClass[wordClass];
            var wordCount = wordList.Count;
            var randomIndex = RandomNumberGenerator.IntegerBetween(0, wordCount);
            return wordList[randomIndex];
        }

        // TODO: could do a one-off processing of the file to avoid this
        private static List<string> ParseResourceWordData(string wordData)
        {
            var words = new List<string>();
            var dataRows = wordData.Split(new[] { NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (var i = FirstDataIndex; i < dataRows.Count; i++)
            {
                var word = dataRows[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).First();
                var isOnlyAlphabeticCharacters = !Regex.IsMatch(word, AnyNonAlphabeticCharactersPattern);
                if (isOnlyAlphabeticCharacters)
                {
                    words.Add(word);
                }
            }

            return words;
        }
    }
}
