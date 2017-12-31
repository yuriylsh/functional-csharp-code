namespace Examples.Purity
{
   public static class StringExt
   {
       public static string ToSentenceCase(this string s) 
           => char.ToUpper(s[0]) + s.Substring(1).ToLower();

       public static string ToSentenceCaseInitial(this string s)
         => s.ToUpper()[0] + s.ToLower().Substring(1);
   }
}