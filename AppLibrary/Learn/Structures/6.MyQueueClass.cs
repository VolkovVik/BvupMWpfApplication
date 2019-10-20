using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <inheritdoc />
    /// <summary>
    /// Очередь
    /// </summary>
    /// <remarks>
    /// Очередь (queue) — это структура данных, представляющая собой
    /// специализированным образом организованный список элементов. 
    /// Доступ к элементам стека осуществляется по принципу FIFO 
    /// (First In First Out) — первым пришел, первым вышел
    /// </remarks>
    public class MyQueueClass< T > : IEnumerable< T > {
        /// <summary>
        /// Количество элементов
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public int Count { get; private set; }

        /// <summary>
        /// Признак заполнения очереди
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Первый (головной) элемент очереди
        /// </summary>
        private Item< T > _head;

        /// <summary>
        /// Последний (хвостовой) элемент очереди
        /// </summary>
        private Item< T > _tail;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MyQueueClass() {
            Clear();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="data"></param>
        public MyQueueClass( T data ) {
            SetHeadItem( data );
        }

        /// <summary>
        /// Задание первого элемента очереди
        /// </summary>
        /// <param name="data"></param>
        private void SetHeadItem( T data ) {
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            var item = new Item< T >( data );
            _head = item;
            _tail = item;
            Count = 1;
        }

        /// <summary>
        /// Подпрограмма сброса данных очереди
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public void Clear() {
            _head = null;
            _tail = null;
            Count = 0;
        }

        /// <summary>
        /// Подпрограмма добавления нового элемента в очередь
        /// </summary>
        /// <param name="data"></param>
        public void Push( T data ) {
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            if ( IsEmpty ) {
                SetHeadItem( data );
                return;
            }
            var item = new Item< T >( data );
            _tail.Next = item;
            _tail      = item;
            Count++;
        }

        /// <summary>
        /// Подпрограмма получения данных c удалением его из очереди
        /// </summary>
        /// <returns></returns>
        public T Pop() {
            if ( IsEmpty ) throw new InvalidOperationException( "Очередь пуста" );
            Count--;
            var output = _head.Data;
            _head = _head.Next;
            return output;
        }

        /// <summary>
        /// Подпрограмма получения данных без удаления его из очереди
        /// </summary>
        /// <returns></returns>
        public T Peek() {
            if ( IsEmpty ) throw new InvalidOperationException( "Очередь пуста" );
            return _head.Data;
        }

        /// <summary>
        /// Подпрограмма возврата строки
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var str     = $"Queue {Count:D3} items : ";
            var current = _head;
            while ( current != null ) {
                str     += current.Data + "  ";
                current =  current.Next;
            }
            return str;
        }

        /// <inheritdoc />
        /// <summary>
        /// Подпрограмма возвращающая перечислитель, выполняющий перебор всех элементов
        /// </summary>
        /// <returns></returns>
        public IEnumerator< T > GetEnumerator() {
            var current = _head;
            while ( current != null ) {
                yield return current.Data;
                current = current.Next;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Подпрограмма возвращающая перечислитель, который осуществляет итерационный переход
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}