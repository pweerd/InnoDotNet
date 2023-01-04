using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InnoCom {

   public interface ILogger {
      void Close ();
      void Log ();
      void Log (string txt);

      void Log (String txt, params object[] args);

      void Log (Exception e);

      void Log (Exception e, String txt);
      void Log (Exception e, String txt, params object[] args);
   }


   public class Logger : ILogger {
      [return: MarshalAs (UnmanagedType.Bool)]
      [DllImport ("kernel32.dll", SetLastError = true)]
      internal static extern bool FlushFileBuffers (SafeFileHandle hFile);

      internal static readonly ILogger dummy = new DummyLogger ();
      private static ILogger? instance = null;
      public static ILogger Instance => instance != null ? instance : createLogger ();
      //public static ILogger Instance => new Logger ();

      private readonly StreamWriter wtr;
      private readonly string name;

      private static ILogger createLogger () {
         try {
            return instance = new Logger ();
         } catch (Exception e) {
            return dummy;
         }
      }

      private Logger () {
         var asm = Assembly.GetExecutingAssembly ();
         name = Path.GetFileNameWithoutExtension(asm.Location);
         var path = Path.GetDirectoryName(Path.GetDirectoryName (asm.Location));
         var fn = String.Format ("InnoExtensions-{0:yyyyMMdd-HHmmss}.Log", DateTime.Now);
         var fs = new FileStream (Path.Combine (path, fn), FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096);
         wtr = new StreamWriter (fs, Encoding.UTF8, 4096);
         Log ("Log opened: {0}, assembly {1}", fn, name);
      }

      public void Close () {
         instance = null;
         try {
            _log ("Log closed");
            wtr.Close ();
         } catch { }
      }

      private void flush() {
         wtr.Flush ();
         FlushFileBuffers (((FileStream)wtr.BaseStream).SafeFileHandle);
      }

      public virtual void Log () {
         wtr.WriteLine ();
         flush ();
      }

      public virtual void Log (String txt) {
         _log (txt);
         flush ();
      }

      public virtual void Log (String txt, params object[] args) {
         _log (txt, args);
         flush ();
      }
      public virtual void Log (Exception e) {
         _log ();
         _log (e.Message);
         _logTrace (e, true);
         _logInner (e.InnerException);
         flush ();
      }

      public virtual void Log (Exception e, String txt) {
         _log ();
         _log ("{0}: {1}", txt, e.Message);
         _logTrace (e, true);
         _logInner (e.InnerException);
         flush ();
      }
      public virtual void Log (Exception e, String txt, params object[] args) {
         Log (e, String.Format (txt, args) + ": " + e.Message);
      }



      private void _log () {
         wtr.WriteLine ();
      }
      private void _log (string txt) {
         wtr.WriteLine ("{0:yyyy-MM-dd hh:mm:ss.fff} - {1} - {2}", DateTime.Now, name, txt);
      }
      private void _log (string txt, params object[] args) {
         _log (String.Format (txt, args));
      }

      private void _logTrace (Exception e, bool autoTrace) {
         String? trace = e.StackTrace;
         if (autoTrace && String.IsNullOrEmpty (trace))
            trace = new StackTrace (2, true).ToString ();
         if (!String.IsNullOrEmpty (trace)) {
            _log ("-- " + trace);
         }
      }

      private void _logInner (Exception? e) {
         if (e == null) return;

         wtr.WriteLine ("--- Inner: " + e.Message);
         _logTrace (e, false);
         _logInner (e.InnerException);
      }
   }


   class DummyLogger : ILogger {
      public void Close () {
      }

      public void Log () {
      }

      public void Log (string txt) {
      }

      public void Log (string txt, params object[] args) {
      }

      public void Log (Exception e) {
      }

      public void Log (Exception e, string txt) {
      }

      public void Log (Exception e, string txt, params object[] args) {
      }
   }
}
