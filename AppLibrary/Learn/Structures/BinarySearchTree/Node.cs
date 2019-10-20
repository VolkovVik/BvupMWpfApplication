using System;

namespace AppLibrary.Learn.Structures.BinarySearchTree {
    internal class Node< T > : IComparable< T >, IComparable where T : IComparable, IComparable< T > {
        public T         Data  { get; private set; }
        public Node< T > Left  { get; private set; }
        public Node< T > Right { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public Node( T data ) {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public Node( T data, Node< T > left, Node< T > right ) {
            Data  = data;
            Left  = left;
            Right = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Add( T data ) {
            var node = new Node< T >( data );
            if ( node.Data.CompareTo( Data ) == -1 ) {
                if ( Left == null ) {
                    Left = node;
                } else {
                    Left.Add( data );
                }
            } else {
                if ( Right == null ) {
                    Right = node;
                } else {
                    Right.Add( data );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Сравнение производится по полю Data.
        /// Метод возвращает 1, если значение текущего узла больше,
        /// чем переданного методу, -1, если меньше и 0, если они равны
        /// </remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo( T other ) {
            return Data.CompareTo( other );
        }

        public int CompareTo( object obj ) {
            if ( obj is Node< T > == false ) throw new ArgumentException( "Не совпадение типов" );
            return Data.CompareTo( ( Node< T > ) obj );
        }

        public override string ToString() {
            return Data.ToString();
        }
    }
}