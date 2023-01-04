using InnoCom;

namespace LoggerTest {
   public partial class Form1 : Form {
      public Form1 () {
         InitializeComponent ();
      }

      private void button1_Click (object sender, EventArgs e) {
         var logger = Logger.Instance;
         logger.Log ("single line");
         logger.Log ("single line with format [{0}]", "some text");
         try {
            throw new Exception ("hola");
         } catch (Exception ex) {
            logger.Log (ex, "Should see 'hola'");
         }
         logger.Close ();
         logger.Close ();

      }
   }
}