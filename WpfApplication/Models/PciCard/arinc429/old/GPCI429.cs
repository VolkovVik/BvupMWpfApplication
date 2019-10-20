using System;
using System.Runtime.InteropServices;

//===== Размышления о трудностях бытия ========================================
// Герасимов Д.А. Размышления о трудностях бытия.
//
// Примечание автора. 
// Я ни где не смог найти чёткого описания процесса передачи данных между
//управляемым и не управляемым кодом. А заниматься этим
// досконально - нет времени. Поэтому все изложенные здесь сведения являются
// моими умозаключениями.
//
// ВСТУПЛЕНИЕ
//
// Передача данных между управляемым и неуправляемым кодом - сложный,
// многогранный и не тривиальный процесс. По нему было прочитано много
// документации. Выводы не утещительны...
// Сначало о хорошом.
// Можно и это успешно работает.
// Теперь о плохом.
// Передача данных сложнейший процесс, имеющий много условностей...
//
// ОБЩИЕ СВЕДЕНИЯ
//
// Опасаный код.
// Можно использовать unsafe код, fixed переменные. Тогда разрешается работать
// с указателями. Однако такой подход противоречит самой идее управляемого кода.
// Требует разрешения компилятору использовать unsafe коде и взваливает на
// програмиста ответственность не только следить за указателями, но еще и
// учитывать особенности работы сборщика мусора платформы .NET. Не будем
// рассматривать данный подход...
//
// Управляемый код (УК)
// Другой подход - использование маршалинга (Marshal). Ооо! Маршалинг это наше
// всё при соединении неуправляемого и управляемого кода. Если вы с ним ещё
// не знакомы - настоятельно рекомендую ознакомиться. Про этот механизм можно
// написать книгу... Здесь же излагаются только основные сведения.
// Как передаются данные из управляемого кода в неуправляемый?
//
// Простые типы.
// Простые типы: int, short, bool и прочие. К ним же относятся и 
// структуры, которые простыми типами язык не поворачивается назвать.
// Похоже механизм передачи - обычная передача через стек.
// Директивы ref и out помогут передавать указатели на соответствующие
// ячейки памяти. Далее дело техники. Возможно передача происходит по тем же
// алгоритмам, что и сложные типы, но нигде точного упоминания об этом не
// обнаружил. Всё просто работает. Когда надо только получить значение -
// используейте описание out. Это ускорит производительность, так как 
// не нужно выполнять задание переменной перед её передачью в подпрограмму.
// Когда надо только передать значение - ничего не используете или используйте
// директиву in Это ускорит производительность, так как не нужно выполнять
// начальную инициализацию возвращаемой переменной.
// Со структурами немного сложнее. Кроме испоьзования директив ref, out,
// надо указывать это компилятору явно порядок порядок объявления данных в структуре
// (см. [StructLayout(LayoutKind.Sequential)] или явное указание смещения
// члена относительно начала структуры.
//
// Сложные типы (структуры, массивы, классы, функции, строки)
// Для сложных типов, использование ref, out не целесообразно, так как при этом
// будут передаваться указатели на указатель. См. описание языка C#.
//
// Явный алгоритм передачи (см. использование типа IntPtr и класса GCHandle).
// 1. Создаётся указатель типа IntPtr;
// 2. Создаётся переменная класса GCHandle (по суте это указатель на любую
// переменную управляемого кода). Этот "указатель" может указывать как на именованный
// объект (существующая переменная), так на неименованный объект (безымянное место в стеке).
// Пока не будет вызван метод GCHandle.Free объект, на которое указывает GCHandle
// Не будет использоваться для других целей и удалятся сборщиком мусора.
// Однако объёект может быть перемещено в другое место. А это уже не приятно.
// неуправляемоя память не умеет работать с перемещаемыми объектами.
// Чтобы этого не происходило надо при создании GCHandle использовать
// тип pinned. К тому же это решает и другую проблемму. О ней далее
// 3. Теперь указателю IntPtr можно присвоить адрес объекта, на который
// указывает GCHandle и передать его в неуправляемый код как указатель.
// 4. Неуправляемой код оперирует по адресам и считывает, записывает данные в
// управляемую переменную. Здесь наш ждёт ещё один "подводный камень" IntPtr
// указывает не на данные (например первый элемент массива), а на объёкт
// управляемого кода. Первые 4 байта содержат служебную информацию. А данные
// начинаются с 5-го байта. А вдруг в будущем изменят эту политику? В этом
// нам поможет признак Pinned. Он не только позволит не перемещять объект, но 
// задать в IntPtr не только адрес объекта, но и адрес данных.
// 5. C помощью команды Free объкт, на который указывает GCHandle возвращается
// в лоно управляемого кода и его сборщика мусора.
// Сложно... Как для понимания, так и для написания. Но работает.
//
// Неявный алгоритм передачи.
// Однако яврный алгоритм передачи не всегда удобен. Особенно при работе со строками.
// Другой подход - использование директив маршалинга. В этом нам помогут
// подсказкам в виде директив [MarshalAs()] и прочих...
//
// Передача строк
// string прекрасно позволяет передать указатель на строку в НК. Можно при
// этом указать (MarshalAs) передавать Юникоды или ANSI. Но вот обрабно получить
// мы ничего не сможем...
// Для получения обратно строк нам поможет класс stringBuilder.
//
// Передача структур.
// Передаются как простые типы. Советую обратить внимание на директиву
// [StructLayout(LayoutKind.Sequential, Pack=X)] Pack - необязательный паараметр
// который задаёт размер выравнивания. Почитайте про выравнивание. Обычно его
// не требуется корректировать, но иногда компилятор управляемого и неуправляемого
// кода действуют не одинаково и тогда начинается ад. Данные располагаются не
// там, где они ожидаются. Надо смотреть дампы памяти и корректировать их.
// В основном это случается при использовании в сложных структурах переменных
// разных типов.
//
// Передача классов
// Не разбирался. Сложно и не однозначно. Чем-то похоже на передачу структур.
//
// Передача массивов.
// С помощью маршалинга Массивы передаются только в одно сторону из УК в НК. И всё!
// Извечный вопрос. Что делать?
// Сами ручками реализуйте явный алгоритм передачи. :-(
//
// ЗАКЛЮЧЕНИЕ
// Вообще у меня сложилось устойчивое мнение.
// Чем реже взаимодействие управляемого и неуправляемого кода, тем для
// производительности лучше.
//====================================================================

namespace WpfApplication.Models.PciCard.arinc429 {
    //===== Структуры для работы с PCI429_4.dll ===============================
    // Структура с информацией о плате PCI429_4
    // Внимание!!! Данная структура содержит служебную информацию, необходимую
    // для правильного функционирования библиотеки. Корректно задаётся и
    // изменяется функциями библиотеки.
    // Ручная корректировка содержимого структуры приведёт к непредсказуемой
    // работе библиотеки.
    //
    // Сообщаем компилятору и маршалингу, что данные в структуре необходимо
    // передовать в порядке их объявления в структуре. Не оптимизировать их
    // размещение.
    /// <summary>
    /// Информацией о плате PCI429_4
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    struct pci429_4_tag {
        /// <summary>
        /// Указатель на плату
        /// </summary>
        public IntPtr handle;

        /// <summary>
        /// Идентификатор модуля
        /// </summary>
        public ushort id;

        /// <summary>
        /// Количество входных каналов
        /// </summary>
        public ushort icn;

        /// <summary>
        /// Количество выходных каналов
        /// </summary>
        public ushort ocn;

        // Сообщаем маршалингу, что на этом месте структуры должны быть не
        // указатель на массив, а его 16 элементов. Однако это не освобождает
        // нас от выделения памяти далее с помощью команды new...
        /// <summary>
        /// Служебный массив указателей.
        /// </summary>
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 16 )]
        public ushort[] last_read_ptr;

        /// <summary>
        /// Массив частот и режимов контроля чётности для вх. и вых. каналов
        /// </summary>
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
        public ushort[] freq;
    };

    /// <summary>
    /// Основные коды ошибок при работе с PCI429_4. Остальные коды аналогичны кодам
    /// ошибок Windows.
    /// </summary>
    public struct pci429_4_errors {
        /// <summary>
        /// Успех
        /// </summary>
        public const uint ERROR_SUCCESS = 0;

        /// <summary>
        /// Не правильный указатель на плату
        /// </summary>
        public const uint ERROR_INVALID_HANDLE = 6;

        /// <summary>
        /// Недостаточно памяти для выполнения операции
        /// </summary>
        public const uint ERROR_NOT_ENOUGH_MEMORY = 8;

        /// <summary>
        /// Один или более параметров функции имеют недопустимое значение
        /// </summary>
        public const uint ERROR_INVALID_PARAMETER = 87;

        /// <summary>
        /// Буфер выдачи не готов
        /// </summary>
        public const uint PCI429_ERROR_TxR_BUSY = 0x4000;

        /// <summary>
        /// Нет новых данных
        /// </summary>
        public const uint PCI429_ERROR_RxD_NO_DATA = 0x4001;

        /// <summary>
        /// Ошибка обмена с драйвером
        /// </summary>
        public const uint PCI429_ERROR_IO_DRIVER = 0x4002;

        /// <summary>
        /// Неправильный указатель на плату
        /// </summary>
        public const uint INVALID_HANDLE_VALUE = 0xFFFFFFFF;
    }

    /// <summary>
    /// Частоты работы каналов
    /// OFF - выкл.
    /// F12 - 12.5 кГц
    /// F50 - 50 кГц
    /// F100 - 100 кГц
    /// </summary>
    public enum FREQ : ushort {
        OFF = 0,
        F12,
        F50,
        F100
    };

    /// <summary>
    /// Класс для работы с платой PCI429-4. Для работы трует библиотеку PCI429_4.dll
    /// </summary>
    public class GPCI429 {
        //===== Переменные класса ==============================================

        /// <summary>
        /// Указатель на плату PCI429-4
        /// </summary>
        public uint handle {
            get {
                lock ( this ) {
                    return ( uint ) ( this.dev.handle );
                }
            }
        }

        /// <summary>
        /// Код последней ошибки при работе платой
        /// </summary>
        private uint error_;

        public uint error {
            get {
                lock ( this ) {
                    return this.error_;
                }
            }
        }

        /// <summary>
        /// Идентификатор платы
        /// </summary>
        private ushort id_;

        public ushort id {
            get {
                lock ( this ) {
                    return this.id_;
                }
            }
        }

        /// <summary>
        /// Командное слово режима, записанное в плату.
        /// </summary>
        private ushort kcp_;

        public ushort kcp {
            get {
                lock ( this ) {
                    return this.kcp_;
                }
            }
        }

        /// <summary>
        /// Информация о плате
        /// </summary>
        private pci429_4_tag dev;

        /// <summary>
        /// Буферы принимаемой информации для каждого из 16 каналов
        /// </summary>
        private uint[][] input_buffer;

        /// <summary>
        /// Индекс возвращаемого слова
        /// </summary>
        private uint[] input_buffer_idx = new uint[16];

        /// <summary>
        /// Индекс максимальное значение индекса возвращаемого слова
        /// </summary>
        private uint[] input_buffer_size = new uint[16];

        //===== Функции класса =================================================
        //Упрощенные и адаптированные аналоги функций из PCI429_4.DLL

        /// <summary>
        /// Инициализация платы. Аналог pci429_4_open(...)
        /// </summary>
        /// <param name="sn">(in) Серийный номер платы</param>
        public GPCI429( ushort sn ) {
            int i;
            // Выделение места для буферов принимаемой информации
            this.input_buffer = new uint[16][];
            for ( i = 0; i < this.input_buffer.Length; i++ ) {
                this.input_buffer[ i ] = new uint[256];
            }
            // Выделение места для информации о плате и задание нач. значений.
            this.dev = new pci429_4_tag {
                handle = ( IntPtr ) unchecked( ( int ) pci429_4_errors.INVALID_HANDLE_VALUE )
            };
            //Info.LastReadPtr = new ushort[16]; // Можно не делать, так как маршалинг при вызове функции из DLL
            //Info.FreqPBK = new ushort[32];     // (неуправляемый код) сделает это за нас.
            // Инициализация платы
            this.error_ = NativeMethods.pci429_4_open( sn, out this.dev );
        }

        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~GPCI429() {
            this.close();
        }

        /// <summary>
        /// Запуск платы и переход в рабочий режим. Аналог pci429_4_start(...)
        /// </summary>
        /// <param name="type">(in) Командное слово режима. См. описание платы.
        /// Значение по умолчанию 0.</param>
        /// <returns>Код ошибки</returns>
        public uint start( ushort newKcp = 0 ) {
            lock ( this ) {
                this.kcp_   = newKcp;
                this.error_ = NativeMethods.pci429_4_start( ref this.dev, this.kcp_ );
                return this.error_;
            }
        }

        /// <summary>
        /// Деинициализация платы. Аналог pci429_4_close(...)
        /// !!! Рекомендуется вызывать перед окончанием работы !!!
        /// </summary>
        /// <returns>Код ошибки</returns>
        public uint close() {
            lock ( this ) {
                if ( ( uint ) ( this.dev.handle ) == pci429_4_errors.INVALID_HANDLE_VALUE ) {
                    this.error_ = pci429_4_errors.ERROR_SUCCESS;
                    return this.error_;
                }
                this.error_ = NativeMethods.pci429_4_close( ref this.dev );
                if ( this.error_ == pci429_4_errors.ERROR_SUCCESS ) {
                    this.dev.handle = ( IntPtr ) pci429_4_errors.ERROR_INVALID_HANDLE;
                }
                return this.error_;
            }
        }

        /// <summary>
        /// Задание частоты работы канала ПБК и режима четности. Аналог
        /// pci429_4_set_freq(...)
        /// </summary>
        /// <param name="type">(in) 1-входной/0-выходной канал</param>
        /// <param name="number">(in) Номер канала для выдачи (допустимые
        /// значения от 1 до 16). Если номер канала выходит за допустимый
        /// диапазон или канал с указанным номером не существует на плате
        /// (информация берётся из PInfo), то генерируется ошибка
        /// ERROR_INVALID_PARAMETER</param>
        /// <param name="freq">(in) Код частота работы канала</param>
        /// <param name="parity_off">(in) false/true - контролировать/не
        /// контролировать бит четности</param>
        /// <returns>Код ошибки</returns>
        public uint set_freq( byte type, byte nk, FREQ freq, bool parity_off ) {
            lock ( this ) {
                this.error_ = NativeMethods.pci429_4_set_freq( ref this.dev, type, nk, freq, parity_off );
                return this.error_;
            }
        }

        /// <summary>
        /// Подпрограмма проверки окончания предыдущей выдачи массива слов.
        /// </summary>
        /// <param name="nk">(in) Номер канала для выдачи (допустимые значения от 1
        /// до 16). Если номер канала выходит за допустимый диапазон или канал с
        /// указанным номером не существует на плате (информация берётся из PInfo),
        /// то генерируется ошибка ERROR_INVALID_PARAMETER</param>
        public uint check_tx( ushort nk ) {
            lock ( this ) {
                this.error_ = NativeMethods.pci429_4_check_tx( ref this.dev, nk );
                return this.error_;
            }
        }

        /// <summary>
        /// Выдать массив слов. Аналог pci429_4_write_tx(...)
        /// </summary>
        /// <param name="nk">(in) Номер канала для выдачи (допустимые значения от 1
        /// до 16). Если номер канала выходит за допустимый диапазон или канал с
        /// указанным номером не существует на плате (информация берётся из PInfo),
        /// то генерируется ошибка ERROR_INVALID_PARAMETER</param>
        /// <param name="buf">(in) Указатель на массив выдаваемых слов</param>
        /// <param name="buf_size">(in) Количество выдаваемых слов (допустимые
        /// значения от 1 до 512). Если количество слов выходит за допустимый
        /// диапазон, то генерируется ошибка ERROR_INVALID_PARAMETER.</param>
        /// <returns>Код ошибки</returns>
        public uint write_tx( ushort nk, uint[] buf, uint buf_size ) {
            lock ( this ) {
                GCHandle gch = GCHandle.Alloc( buf, GCHandleType.Pinned );
                this.error_ = NativeMethods.pci429_4_write_tx( ref this.dev, nk, gch.AddrOfPinnedObject(), buf_size );
                gch.Free();
                return this.error_;
            }
        }

        /// <summary>
        /// Выдать слово. Аналог pci429_4_write_tx(...)
        /// </summary>
        /// <param name="nk">(in) Номер канала для выдачи (допустимые значения от 1
        /// до 16). Если номер канала выходит за допустимый диапазон или канал с
        /// указанным номером не существует на плате (информация берётся из PInfo),
        /// то генерируется ошибка ERROR_INVALID_PARAMETER</param>
        /// <param name="word">(in) Указатель на массив выдаваемых слов</param>
        /// <returns>Код ошибки</returns>
        public uint write_tx( ushort nk, uint word ) {
            lock ( this ) {
                GCHandle gch = GCHandle.Alloc( word, GCHandleType.Pinned );
                this.error_ = NativeMethods.pci429_4_write_tx( ref this.dev, nk, gch.AddrOfPinnedObject(), 1 );
                gch.Free();
                return this.error_;
            }
        }

        /// <summary>
        /// Считать принятое слово. Аналог pci429_4_read_rx(...)
        /// ПРИМЕЧАНИЕ - !!! Так как в плате нет средств контроля за переполнением
        /// буфера приема данная программа не будет работать корректно, если частота
        /// считывания слов из платы, ниже частоты приема.
        /// </summary>
        /// <param name="nk">(in) Номер канала для выдачи (допустимые значения от 1
        /// до 16). Если номер канала выходит за допустимый диапазон или канал с
        /// указанным номером не существует на плате (информация берётся из PInfo),
        /// то генерируется ошибка ERROR_INVALID_PARAMETER</param>
        /// <param name="word">(out) Принятое слово</param>
        /// <returns>Код ошибки</returns>
        public uint read_rx( ushort nk, out uint word ) {
            word = 0;
            lock ( this ) {
                // Проверка допустимости номера канала
                if ( !( ( 0 < nk ) && ( nk <= this.dev.icn ) ) ) {
                    //Неверное число каналов
                    return pci429_4_errors.ERROR_INVALID_PARAMETER;
                }
                nk--;      //Перевод номера канала в индекс массива
                ReadAgain: // Если буфер входных параметров не пуст возвращается очередное
                // слово из буфера.
                if ( this.input_buffer_size[ nk ] != 0 ) {
                    // Буфер не пуст
                    word = this.input_buffer[ nk ][ this.input_buffer_idx[ nk ] ];
                    this.input_buffer_idx[ nk ]++;
                    // Если индекс возвращаемого значения равен максимальному
                    // значению, то весь буфер считан. Устанавливаем признак
                    // буфер пуст.
                    if ( this.input_buffer_idx[ nk ] == this.input_buffer_size[ nk ] ) {
                        //Буфер пуст
                        this.input_buffer_size[ nk ] = 0;
                        this.input_buffer_idx[ nk ]  = 0;
                    }
                    return pci429_4_errors.ERROR_SUCCESS;
                }
                // Буфер пуст.
                // Считываем новые слова из платы.
                this.input_buffer_size[ nk ] = ( uint ) ( this.input_buffer[ nk ] ).Length;
                GCHandle gch = GCHandle.Alloc( this.input_buffer[ nk ], GCHandleType.Pinned );
                this.error_ = NativeMethods.pci429_4_read_rx( ref this.dev, ( ushort ) ( nk + 1 ),
                    gch.AddrOfPinnedObject(),
                    ref this.input_buffer_size[ nk ] );
                gch.Free();
                if ( this.error_ == pci429_4_errors.ERROR_SUCCESS ) {
                    //Новые данные получены.
                    goto ReadAgain;
                }
                // Нет новых данных или ошибка работы с платой.
                return this.error_;
            }
        }

        /// <summary>
        /// Чтение слова состояния устройства. Аналог pci429_4_get_state(...)
        /// Если указатель на любой выходной параметров равен null, то значение
        /// данного параметра не возвращается.
        /// </summary>
        /// <param name="prev_mode">(out) Предыдущий режим работы (3000h МР  ?пред-слово программы? - отражает предыдущий режим выполнения программы модуля)</param>
        /// <param name="currentMode">(out) Текущий режим работы (3001h РМ  ?Слово состояния программы? - отражает текущий режим программы модуля)</param>
        /// <param name="version">(out) Версия ПО в плате (3002h ВПМ ?Код версии программы?- два байта цифровых символов ASCI кода, пример: 3031=01)</param>
        /// <param name="testResult">(out) Результат встроенного контроля (3003h ВСК ?Слово состояния контроля? модуля по тесту ВСК)</param>
        /// <returns>Код ошибки.</returns>
        public uint get_state( ref ushort prev_mode, out ushort currentMode, out ushort version,
            out ushort testResult ) {
            lock ( this ) {
                this.error_ = NativeMethods.pci429_4_get_state( ref this.dev, out prev_mode, out currentMode,
                    out version,
                    out testResult );
                return this.error_;
            }
        }

        /// <summary>
        /// Сброс модуля. Аналог pci429_4_reset(...)
        /// </summary>
        /// <returns>Код ошибки</returns>
        public uint reset() {
            lock ( this ) {
                this.id_    = 0;
                this.error_ = NativeMethods.pci429_4_reset( ref this.dev, out this.id_ );
                if ( this.error_ == pci429_4_errors.ERROR_SUCCESS ) {
                    // Сброс буферов приемника
                    for ( var nk = 0; nk < 16; nk++ ) {
                        //Буфер пуст
                        this.input_buffer_size[ nk ] = 0;
                        this.input_buffer_idx[ nk ]  = 0;
                    }
                }
                return this.error_;
            }
        }

        /// <summary>
        /// Считывание серийного номера платы из регистра платы.
        /// Аналог pci429_4_get_serial_number(...)
        /// </summary>
        /// <param name="sn">(out) Серийный номер платы</param>
        /// <returns>Код ошибки</returns>
        public uint get_serial_number( out ushort sn ) {
            lock ( this ) {
                this.error_ = NativeMethods.pci429_4_get_serial_number( ref this.dev, out sn );
                return this.error_;
            }
        }

        /// <summary>
        /// Считывание адресов платы. Аналог pci429_4_get_address(...)
        /// Если указатель на выходную переменную равен 0 (NULL), то значение
        /// данного параметра не возвращается.
        /// </summary>
        /// <param name="PLX_address">(out) Адрес портов PLX</param>
        /// <param name="userAddress">(out) Адреса пользовательских портов</param>
        /// <param name="irq">(out) Номер прерывания</param>
        /// <returns>Код ошибки</returns>
        public uint get_address( out ushort PLX_address, out ushort userAddress, out ushort irq ) {
            lock ( this ) {
                this.error_ = NativeMethods.pci429_4_get_address( ref this.dev, out PLX_address, out userAddress,
                    out irq );
                return this.error_;
            }
        }

        /// <summary>
        /// Записать регистр выходных разовых команд. Аналог pci429_4_write_rk(...)
        /// </summary>
        /// <param name="rk">(in) Данные для записи</param>
        /// <returns>Код ошибки</returns>
        public uint write_rk( ushort rk ) {
            lock ( this ) {
                this.error_ = NativeMethods.pci429_4_write_rk( ref this.dev, rk );
                return this.error_;
            }
        }

        /// <summary>
        /// Считать регистр входных разовых команд. Аналог pci429_4_read_rk(...)
        /// </summary>
        /// <param name="rk">(out) Считанные данные</param>
        /// <returns>Код ошибки</returns>
        public uint read_rk( out ushort rk ) {
            lock ( this ) {
                this.error_ = NativeMethods.pci429_4_read_rk( ref this.dev, out rk );
                return this.error_;
            }
        }
    }

    /// <summary>
    /// Класс, содержащий функции для работы с PCI429_4.dll
    /// CA1060: переместите P/Invokes в класс NativeMethods
    /// </summary>
    internal static class NativeMethods {
        /// <summary>
        /// Чтение слова состояния устройства.
        /// </summary>
        /// <param name="dev">(in) Информация об устройстве</param>
        /// <param name="prev_mode">(out) Предыдущий режим работы (3000h МР  ?пред-слово программы? - отражает предыдущий режим выполнения программы модуля)</param>
        /// <param name="currentMode">(out) Текущий режим работы (3001h РМ  ?Слово состояния программы? - отражает текущий режим программы модуля)</param>
        /// <param name="version">(out) Версия ПО в плате (3002h ВПМ ?Код версии программы?- два байта цифровых символов ASCI кода, пример: 3031=01)</param>
        /// <param name="testResult">(out) Результат встроенного контроля (3003h ВСК ?Слово состояния контроля? модуля по тесту ВСК)</param>
        /// <returns>Код ошибки.</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_get_state( ref pci429_4_tag dev,
            out ushort prev_mode,
            out ushort currentMode, out ushort version, out ushort testResult );

        /// <summary>
        /// Сброс модуля и чтение идентификатора модуля.
        /// При чтении регистра в PCI передается код ИД() идентификатора модуля и аппаратно формируется сигнал сброса модуля, при этом устанавливаются биты регистра RM(14,13,11), обнуляется бит регистра РМ(15) и регистры РВ, РДВ, снимаются все блокировки и процессор модуля переходит в режим программной инициализации модуля.
        /// ИД(15-8)- содержит код модуля; ИД(7-4)- код модификации модуля и ИД(3-0)- код версии модуля;
        /// Модуль изготавливается в следующих модификациях по количеству каналов ПК:
        /// PCI429-4-1 - восемь входных (КП1..КП8), восемь выходных (КВ1..КВ8) каналов ПК, код ИД=4010h;
        /// PCI429-4-2 - шестнадцать входных, восемь выходных (КВ1..КВ8) каналов ПК, код ИД=4020h;
        /// PCI429-4-3 - шестнадцать входных, шестнадцать выходных каналов ПК, код ИД=4030h.
        /// </summary>
        /// <param name="dev">(in) Информация об устройстве</param>
        /// <param name="id">(out) Идентификатор модуля</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_reset( ref pci429_4_tag dev, out ushort id );

        /// <summary>
        /// Инициализация платы.
        /// </summary>
        /// <param name="sn">(in) Cерийный номер платы</param>
        /// <param name="dev">(out) Информация об устройстве</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll", BestFitMapping = true )]
        internal static extern uint pci429_4_open( ushort sn,
            out pci429_4_tag dev );

        /// <summary>
        /// Деинициализация платы.
        /// </summary>
        /// <param name="dev">(in) Информация об устройстве</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_close( ref pci429_4_tag dev );

        /// <summary>
        /// Считывание серийного номера платы из регистра платы
        /// </summary>
        /// <param name="dev">(in) Информация об устройстве</param>
        /// <param name="sn">(out) Серийный номер платы</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_get_serial_number( ref pci429_4_tag dev,
            out ushort sn );

        /// <summary>
        /// Считывание информации об адресах платы
        /// </summary>
        /// <param name="dev">(in) Информация об устройстве</param>
        /// <param name="PLX_address">(out) Адрес портов PLX</param>
        /// <param name="userAddress">(out) Адреса пользовательских портов</param>
        /// <param name="irq">(out) Номер прерывания</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_get_address( ref pci429_4_tag dev,
            out ushort PLX_address, out ushort userAddress, out ushort irq );

        /// <summary>
        /// Запуск платы и переход в рабочий режим.
        /// </summary>
        /// <param name="dev">(in/out) Информация об устройстве</param>
        /// <param name="kcp">(in) Командное слово режима. ( Рекомендуемое значение 0 )</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_start( ref pci429_4_tag dev, ushort kcp );

        /// <summary>
        /// Проверка окончания предыдущей выдачи массива слов
        /// </summary>
        /// <param name="dev">(in) Информация об устройстве</param>
        /// <param name="nk">(in) Номер канала для выдачи (допустимые значения от 1 до 16). Если номер канала выходит за допустимый
        /// диапазон или канал с указанным номером не существует на плате (информация берётся из PInfo), то
        /// генерируется ошибка ERROR_INVALID_PARAMETER</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_check_tx( ref pci429_4_tag dev, ushort nk );

        /// <summary>
        /// Выдать массив слов
        /// </summary>
        /// <param name="dev">(in) Информация об устройстве</param>
        /// <param name="nk">(in) Номер канала для выдачи (допустимые значения от 1 до 16). Если номер канала выходит за допустимый
        /// диапазон или канал с указанным номером не существует на плате (информация берётся из PInfo), то
        /// генерируется ошибка ERROR_INVALID_PARAMETER</param>
        /// <param name="buf">(in) Указатель на массив выдаваемых слов</param>
        /// <param name="buf_size">(in) Количество выдаваемых слов (допустимые значения от 1 до 512). Если количество слов выходит за
        /// допустимый диапазон, то генерируется ошибка ERROR_INVALID_PARAMETER.</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_write_tx( ref pci429_4_tag dev, ushort nk,
            IntPtr buf, uint buf_size );

        /// <summary>
        /// Считать принятые слова
        /// ПРИМЕЧАНИЯ:
        /// - Если задать buf_size=0, то будет возврашена ошибка ERROR_NOT_ENOUGH_MEMORY, а в переменную
        ///   BufSize будет записано количество слов, которые могут быть считаны.
        /// - !!! Так как в плате нет средств контроля за переполнением буфера приема данная программа
        /// не будет работать корректно, если частота считывания слов из платы, ниже частоты приема.
        /// </summary>
        /// <param name="dev">(in/out) Информация об устройстве</param>
        /// <param name="nk">(in) номер считываемого канала приёма (допустимые значения от 1 до 16). Если номер канала выходит за допустимый
        /// диапазон или канал с указанным номером не существует на плате (информация берётся из PInfo), то
        /// генерируется ошибка ERROR_INVALID_PARAMETER</param>
        /// <param name="buf">(in/out) Указатель массив, а котором будут сохранены принятые слова</param>
        /// <param name="buf_size">(in/out) Размер массива в словах (одно слово ARINC - 4 байта)</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_read_rx( ref pci429_4_tag dev, ushort nk,
            IntPtr buf, ref uint buf_size );

        /// <summary>
        /// Задание частоты работы канала ПБК и режима четности.
        /// </summary>
        /// <param name="dev">(in/out) Информация об устройстве</param>
        /// <param name="type">(in) 0-входной/1-выходной канал</param>
        /// <param name="nk">(in) Номер канала для выдачи (допустимые значения от 1 до 16). Если номер канала выходит за допустимый
        /// диапазон или канал с указанным номером не существует на плате (информация берётся из PInfo), то
        /// генерируется ошибка ERROR_INVALID_PARAMETER</param>
        /// <param name="freq">(in) Код частота работы канала</param>
        /// <param name="parity_off">(in) false/true - контролировать/не контролировать бит четности</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_set_freq( ref pci429_4_tag dev, byte type,
            byte nk, FREQ freq, bool parity_off );

        /// <summary>
        /// Записать регистр выходных разовых команд.
        /// </summary>
        /// <param name="dev">(in/out) Информация об устройстве</param>
        /// <param name="rk">(in) Данные для записи</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_write_rk( ref pci429_4_tag dev, ushort rk );

        /// <summary>
        /// Считать регистр входных разовых команд.
        /// </summary>
        /// <param name="dev">(in/out) Информация об устройстве</param>
        /// <param name="rk">(out) Считанные данные</param>
        /// <returns>Код ошибки</returns>
        [DllImport( "PCI429_4.dll" )]
        internal static extern uint pci429_4_read_rk( ref pci429_4_tag dev, out ushort rk );
    }
}