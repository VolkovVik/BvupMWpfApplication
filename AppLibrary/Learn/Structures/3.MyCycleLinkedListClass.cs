using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <inheritdoc />
    /// <summary>
    /// Кольцевой односвязный список
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyCycleLinkedListClass< T > : IEnumerable< T > {
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
        /// Первый (головной) элемент списка.
        /// </summary>
        private Item< T > _head;

        /// <summary>
        /// Крайний (хвостовой) элемент списка. 
        /// </summary>
        private Item< T > _tail;

        /// <summary>
        /// 
        /// </summary>
        public MyCycleLinkedListClass() {
            Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public MyCycleLinkedListClass( T data ) {
            SetHeadItem( data );
        }

        /// <summary>
        /// Задание первого элемента
        /// </summary>
        /// <param name="data"></param>
        private void SetHeadItem( T data ) {
            var item = new Item< T >( data );
            _head      = item;
            _head.Next = item;
            _tail      = item;
            Count      = 1;
        }

        /// <summary>
        /// Очистить список.
        /// </summary>
        public void Clear() {
            _head = null;
            _tail = null;
            Count = 0;
        }

        /// <summary>
        /// Добавить данные в связный список.
        /// </summary>
        /// <param name="data"></param>
        public void Add( T data ) {
            // Не забываем проверять входные аргументы на null.
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // Если связный список пуст, то добавляем созданный элемент в начало,
            // иначе добавляем этот элемент как следующий за крайним элементом.
            if ( IsEmpty ) {
                SetHeadItem( data );
                return;
            }
            var item = new Item< T >( data ) {
                Next = _head
            };
            _tail.Next = item;
            _tail      = item;
            Count++;
        }

        /// <summary>
        /// Удалить данные из связного списка.
        /// Выполняется удаление первого вхождения данных.
        /// </summary>
        /// <param name="data"> Данные, которые будут удалены </param>
        /// <param name="all"></param>
        public void Delete( T data, bool all = false ) {
            // Не забываем проверять входные аргументы на null.
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // Текущий обозреваемый элемент списка.
            var current = _head;
            // Предыдущий элемент списка, перед обозреваемым.
            Item< T > previous = null;
            // Цикл по всех элементам списка
            var count = Count;
            while ( count > 0 ) {
                count--;
                if ( current == null ) break;
                // Поиск заданного элемента
                if ( !current.Data.Equals( data ) ) {
                    // Элемент не равен заданному
                    // Переходим к следующему элементу списка.
                    previous = current;
                    current  = current.Next;
                } else {
                    // Элемент равен заданному
                    // Удаляем текущий элемент
                    Count--;
                    // Изменение списка для головного и последубщих элементов несколько отличается
                    if ( previous == null ) {
                        if ( current.Next == _head ) {
                            // Список пуст
                            Clear();
                        } else {
                            _head      = current.Next;
                            _tail.Next = _head;
                        }
                    } else {
                        // Устанавливаем у предыдущего элемента указатель
                        // на следующий элемент от текущего.
                        previous.Next = current.Next;
                        if ( previous.Next == _head ) {
                            // Удалили крайний элемент списка
                            // Список пройден до конца
                            _tail = previous;
                        }
                    }
                    current = previous == null ? _head : previous.Next;
                    if ( !all ) break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var str     = $"CycleLinkedList {Count:D3} items : ";
            var current = _head;
            do {
                if ( current == null ) break;
                str     += current.Data + "  ";
                current =  current.Next;
            } while ( current != _head );
            return str;
        }

        /// <inheritdoc />
        /// <summary>
        /// Вернуть перечислитель, выполняющий перебор всех элементов
        /// </summary>
        /// <returns></returns>
        public IEnumerator< T > GetEnumerator() {
            var current = _head;
            do {
                if ( current == null ) break;
                yield return current.Data;
                current = current.Next;
            } while ( current != _head );
        }

        /// <inheritdoc />
        /// <summary>
        /// Вернуть перечислитель, который осуществляет итерационный переход
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}