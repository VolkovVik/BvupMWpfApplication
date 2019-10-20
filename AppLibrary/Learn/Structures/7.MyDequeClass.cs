using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <inheritdoc />
    /// <summary>
    /// Дек
    /// </summary>
    /// <remarks>
    /// Дек (deque) представляет двустороннюю очередь, 
    /// в которой элементы можно добавлять как в начало, 
    /// так и в конец. Удаление также может идти как с 
    /// начала, так и с конца
    /// </remarks>
    public class MyDequeClass< T > : IEnumerable< T > {
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
        /// Свойство получения первого элемента
        /// </summary>
        public T First {
            get {
                if ( IsEmpty ) throw new InvalidOperationException( "Deque empty" );
                return _head.Data;
            }
        }

        /// <summary>
        /// Свойство получения последнего элемент
        /// </summary>
        public T Last {
            get {
                if ( IsEmpty ) throw new InvalidOperationException( "Deque empty" );
                return _tail.Data;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public MyDequeClass() {
            Clear();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="data"></param>
        public MyDequeClass( T data ) {
            SetHeadItem( data );
        }

        /// <summary>
        /// Задание первого элемента очереди
        /// </summary>
        /// <param name="data"></param>
        private void SetHeadItem( T data ) {
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
        /// Подпрограмма добавления нового элемента cначала
        /// </summary>
        /// <param name="data"></param>
        public void PushFirst( T data ) {
            // Проверка входных параметров
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            if ( IsEmpty ) {
                SetHeadItem( data );
                return;
            }
            // Создаем новый элемент
            var item = new Item< T >( data ) {
                Next     = _head,
                Previous = null
            };
            _head.Previous = item;
            _head          = item;
            Count++;
        }

        /// <summary>
        /// Подпрограмма добавления нового элемента cконца
        /// </summary>
        /// <param name="data"></param>
        public void PushLast( T data ) {
            // Проверка входных параметров
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            if ( IsEmpty ) {
                SetHeadItem( data );
                return;
            }
            // Создаем новый элемент
            var item = new Item< T >( data ) {
                Next     = null,
                Previous = _tail
            };
            _tail.Next = item;
            _tail      = item;
            Count++;
        }

        /// <summary>
        /// Подпрограмма получения данных c удалением его из начала
        /// </summary>
        /// <returns></returns>
        public T PopFirst() {
            if ( IsEmpty ) throw new InvalidOperationException( "Deque empty" );
            Count--;
            var output = _head.Data;
            if ( _head.Next == null ) {
                Clear();
            } else {
                _head          = _head.Next;
                _head.Previous = null;
            }
            return output;
        }

        /// <summary>
        /// Подпрограмма получения данных c удалением его из конца
        /// </summary>
        /// <returns></returns>
        public T PopLast() {
            if ( IsEmpty ) throw new InvalidOperationException( "Deque empty" );
            Count--;
            var output = _tail.Data;

            if ( _tail.Previous == null ) {
                Clear();
            } else {
                _tail      = _tail.Previous;
                _tail.Next = null;
            }
            return output;
        }

        /// <summary>
        /// Подпрограмма получения данных c удалением его из начала
        /// </summary>
        /// <returns></returns>
        public T PeekFirst() {
            if ( IsEmpty ) throw new InvalidOperationException( "Deque empty" );
            return _head.Data;
        }

        /// <summary>
        /// Подпрограмма получения данных c удалением его из конца
        /// </summary>
        /// <returns></returns>
        public T PeekLast() {
            if ( IsEmpty ) throw new InvalidOperationException( "Deque empty" );
            return _tail.Data;
        }

        /// <summary>
        /// Подпрограмма возврата строки
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var str     = $"Deque {Count:D3} items : ";
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable< T > BackEnumerator() {
            var current = _tail;
            while ( current != null ) {
                yield return current.Data;
                current = current.Previous;
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