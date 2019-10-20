using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLibrary.Learn.Structures {
    /// <summary>
    /// Связный список
    /// </summary>
    /// <remarks>
    /// Связный список (Linked List) представляет собой коллекцию связанных 
    /// элементов, которые содержат в себе хранимые данные, а также ссылку 
    /// на связанные с ним элементы (один или несколько). Основным 
    /// преимуществом данной структуры данных перед обычным массивом 
    /// является ее динамичность — возможность легко менять количество 
    /// элементов.
    /// Для начала необходимо упомянуть, что существует несколько видов связных списков. Вот наиболее часто используемые из них:
    ///     Односвязный список
    ///     Двусвязный список
    ///     Кольцевой односвязный список
    ///     Кольцевой двусвязный список
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class MyLinkedListClass< T > : IEnumerable< T > {
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
        public MyLinkedListClass() {
            Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public MyLinkedListClass( T data ) {
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
            var item = new Item< T >( data );
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
            while ( current != null ) {
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
                        _head = current.Next;
                        if ( _head == null ) {
                            // Список пуст
                            Clear();
                        }
                    } else {
                        // Устанавливаем у предыдущего элемента указатель
                        // на следующий элемент от текущего.
                        previous.Next = current.Next;
                        if ( previous.Next == null ) {
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
            var str     = $"LinkedList {Count:D3} items : ";
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