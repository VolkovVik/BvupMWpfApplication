using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <inheritdoc />
    /// <summary>
    /// Стек
    /// </summary>
    /// <remarks>
    /// Стек (stack) — это структура данных, представляющая собой 
    /// специализированным образом организованный список элементов. 
    /// Доступ к элементам стека осуществляется по принципу LIFO 
    /// (Last In First Out) — последним пришел, первым вышел. 
    /// </remarks>
    public class MyStackClass< T > : IEnumerable< T > {
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
        /// Первый (головной) элемент стека
        /// </summary>
        private Item< T > _head;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MyStackClass() {
            Clear();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="data"></param>
        public MyStackClass( T data ) {
            Push( data );
        }

        /// <summary>
        /// Подпрограмма сброса данных стека
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public void Clear() {
            _head = null;
            Count = 0;
        }

        /// <summary>
        /// Подпрограмма добавления нового элемента в стек
        /// </summary>
        /// <param name="data"></param>
        public void Push( T data ) {
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // увеличиваем стек
            // переустанавливаем верхушку стека на новый элемент
            var item = new Item< T >( data ) {
                Next = _head
            };
            _head = item;
            Count++;
        }

        /// <summary>
        /// Подпрограмма получения данных c удалением его из стека
        /// </summary>
        /// <returns></returns>
        public T Pop() {
            // если стек пуст, выбрасываем исключение
            if ( IsEmpty ) throw new InvalidOperationException( "Stack empty" );
            Count--;
            var data = _head;
            // Сброс верхнего элемента
            // переустанавливаем верхушку стека на следующий элемент
            _head = _head.Next;
            return data.Data;
        }

        /// <summary>
        /// Подпрограмма получения данных без удаления его из стека
        /// </summary>
        /// <returns></returns>
        public T Peek() {
            if ( IsEmpty ) throw new InvalidOperationException( "Stack empty" );
            return _head.Data;
        }

        /// <summary>
        /// Подпрограмма возврата строки
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var str     = $"Stack {Count:D3} items : ";
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