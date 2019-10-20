using System;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures.BinarySearchTree {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Двоичное дерево поиска строится по определенным правилам:
    /// 1. У каждого узла не более двух детей.
    /// 2. Любое значение меньше значения узла становится левым узлом.
    /// 3. Любое значение больше или равное значению узла становится правым узлом.
    /// Обход в глубину
    /// Сперава разберемся с его видами: 
    /// Прямой обход( CLR – center, left, right). Сначала берется значение корня, затем обходится левое поддерево, затем правое;
    /// Концевой обход( RCL – right, center, left). Сначала обходится правое поддерево, затем берется значение корня, затем левое;
    /// Обратный обход( LCR – left, center, right). Сначала обходится левое поддерево, затем берется значение корня, затем правое поддерево.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    internal class Tree< T > where T : IComparable, IComparable< T > {
        public Node< T > Root  { get; private set; }
        public int       Count { get; private set; }

        public void Add( T data ) {
            if ( Root == null ) {
                Root = new Node< T >( data );
            } else {
                Root.Add( data );
            }
            Count++;
        }

        public void AddRange( IEnumerable< T > data ) {
            foreach ( var item in data ) {
                Add( item );
            }
        }

        /// <summary>
        /// Метод префиксного обхода
        /// </summary>
        /// <remarks>
        /// Префиксный обход обычно применяется для копирования дерева с сохранением его структуры
        /// 1. узел
        /// 2. левое дерево
        /// 3. правое дерево
        /// </remarks>
        /// <returns></returns>
        public List< T > Preorder() {
            return Root == null ? new List< T >() : Preorder( Root );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List< T > Preorder( Node< T > node ) {
            var list = new List< T >();
            if ( node == null ) return list;
            list.Add( node.Data );
            list.AddRange( Preorder( node.Left ) );
            list.AddRange( Preorder( node.Right ) );
            return list;
        }

        /// <summary>
        /// Метод постфиксного обхода
        /// </summary>
        /// <remarks>
        /// Постфиксный обход часто используется для полного удаления дерева,
        /// так как в некоторых языках программирования необходимо убирать 
        /// из памяти все узлы явно, или для удаления поддерева
        /// 1. левое дерево
        /// 2. правое дерево
        /// 3. узел
        /// </remarks>
        /// <returns></returns>
        public List< T > Postorder() {
            return Root == null ? new List< T >() : Postorder( Root );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List< T > Postorder( Node< T > node ) {
            var list = new List< T >();
            if ( node == null ) return list;
            list.AddRange( Postorder( node.Left ) );
            list.AddRange( Postorder( node.Right ) );
            list.Add( node.Data );
            return list;
        }

        /// <summary>
        /// Метод инфиксного обхода
        /// </summary>
        /// <remarks>
        /// Обход применяется для сортировки дерева
        /// 1. левое дерево
        /// 2. узел
        /// 3. правое дерево
        /// </remarks>
        /// <returns></returns>
        public List< T > Inorder() {
            return Root == null ? new List< T >() : Inorder( Root );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List< T > Inorder( Node< T > node ) {
            var list = new List< T >();
            if ( node == null ) return list;
            list.AddRange( Inorder( node.Left ) );
            list.Add( node.Data );
            list.AddRange( Inorder( node.Right ) );
            return list;
        }
    }
}