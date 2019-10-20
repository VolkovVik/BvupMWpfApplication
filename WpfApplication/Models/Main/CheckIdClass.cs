namespace WpfApplication.Models.Main {
    public class CheckIdClass {
        public bool Rs232 { set; get; }
        public bool Rk    { set; get; }
        public bool Dac   { set; get; }
        public bool Adc   { set; get; }
        public bool Arinc { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sign"></param>
        public CheckIdClass( bool sign = false ) {
            Rs232 = Rk = Dac = Adc = Arinc = sign;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sign"></param>
        public void Set( bool sign ) {
            Rs232 = Rk = Dac = Adc = Arinc = sign;
        }
    }
}