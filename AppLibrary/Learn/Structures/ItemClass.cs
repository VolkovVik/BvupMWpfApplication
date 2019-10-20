namespace AppLibrary.Learn.Structures {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Item< T > {
        /// <summary>
        /// Данные
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public T Data { get; private set; }

        /// <summary>
        /// Указатель на следующий элемент
        /// </summary>
        public Item< T > Next { get; set; }

        /// <summary>
        /// Указатель на предыдущий элемент
        /// </summary>
        public Item< T > Previous { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public Item( T data ) {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Data.ToString();
        }
    }
}