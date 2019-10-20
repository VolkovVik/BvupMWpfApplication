using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <inheritdoc />
    /// <summary>
    /// Двусвязный список
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyDoubleLinkedListClass< T > : IEnumerable< T > {
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
        public MyDoubleLinkedListClass() {
            Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public MyDoubleLinkedListClass( T data ) {
            SetHeadItem( data );
        }

        /// <summary>
        /// Задание первого элемента
        /// </summary>
        /// <param name="data"></param>
        private void SetHeadItem( T data ) {
            var item = new Item< T >( data );
            _head = item;
            _tail = item;
            Count = 1;
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
            // Создаем новый элемент связного списка.
            var item = new Item< T >( data ) {
                Previous = _tail
            };
            _tail.Next = item;
            _tail      = item;
            Count++;
        }

        /// <summary>
        /// Добавить данные в связный список.
        /// </summary>
        /// <param name="data"></param>
        public void AddFirst( T data ) {
            // Не забываем проверять входные аргументы на null.
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // Если связный список пуст, то добавляем созданный элемент в начало,
            // иначе добавляем этот элемент как следующий за крайним элементом.
            if ( IsEmpty ) {
                SetHeadItem( data );
                return;
            }
            // Создаем новый элемент связного списка.
            var item = new Item< T >( data ) {
                Next     = _head,
                Previous = null
            };
            _head.Previous = item;
            _head          = item;
            Count++;
        }

        /// <summary>
        /// Удалить данные из связного списка.
        /// Выполняется удаление первого вхождения данных.
        /// </summary>
        /// <param name="data"> Данные, которые будут удалены </param>
        /// <param name="deleteAll"></param>
        public void Delete( T data, bool deleteAll = false ) {
            // Не забываем проверять входные аргументы на null.
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            // Текущий обозреваемый элемент списка.
            var current = _head;
            // Цикл по всех элементам списка
            while ( current != null ) {
                // Поиск заданного элемента
                if ( !current.Data.Equals( data ) ) {
                    // Элемент не равен заданному
                    // Переходим к следующему элементу списка.
                    current = current.Next;
                } else {
                    // Элемент равен заданному
                    // Удаляем текущий элемент
                    Count--;
                    // Изменение списка для головного и последубщих элементов несколько отличается
                    if ( current.Previous == null ) {
                        _head = current.Next;
                        if ( _head == null ) {
                            // Список пуст
                            Clear();
                        } else {
                            _head.Previous = null;
                        }
                    } else {
                        // Устанавливаем у предыдущего элемента указатель
                        // на следующий элемент от текущего.
                        if ( current.Next == null ) {
                            // Удалили крайний элемент списка
                            // Список пройден до конца
                            _tail      = current.Previous;
                            _tail.Next = null;
                        } else {
                            var item = current;
                            current.Previous.Next = item.Next;
                            current.Next.Previous = item.Previous;
                        }
                    }
                    current = current.Previous == null ? _head : current.Previous.Next;
                    if ( !deleteAll ) break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var str     = $"DoubleLinkedList {Count:D3} items : ";
            var current = _head;
            while ( current != null ) {
                str     += current.Data + "  ";
                current =  current.Next;
            }
            return str;
        }

        /// <inheritdoc />
        /// <summary>
        /// Вернуть перечислитель, выполняющий перебор всех элементов
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
        /// Вернуть перечислитель, который осуществляет итерационный переход
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}