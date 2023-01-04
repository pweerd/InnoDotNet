using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace InnoCom {

   [ComVisible (true)]
   [InterfaceType (ComInterfaceType.InterfaceIsIDispatch)]
   public interface Class1Disp {
      int Get42 ();
      String MethodWithException (String x);
      void Close ();
   }


   [ComVisible (true)]
   [Guid ("1E5C0354-FD92-4C3A-8D93-28FAB41A6CBA")]
   [ClassInterface (ClassInterfaceType.None)]
   public class Class1 : Class1Disp {
      public void Close () {
         Logger.Instance.Close ();
      }

      public int Get42 () {
         return 43;
      }

      public string MethodWithException (string x) {
         try {
            if (String.IsNullOrEmpty (x)) throw new Exception ("Empty strings not allowed");
            return x;
         } catch (Exception e) {
            Logger.Instance.Log (e, "MethodWithException");
            throw;
         }
      }

   }

}