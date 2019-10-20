using System;
using WpfApplication.ViewModels;

namespace WpfApplication.Models.Main {
    /// <summary>
    /// Класс конфигурации тестов интерфейсов
    /// </summary>
    public class ConfigTestsClass {
        /// <summary>
        /// Идентификатор задач
        /// </summary>
        public enum IdTest {
            Rkv1 = 1,
            Rkp1,
            Place1,
            Led1,
            A429Rx1,
            A429Tx1,
            A429Echo1,
            A429Load1,
            Dac1,
            Adc1,
            Doz1,
            Doz2,
            SupplyVoltage1,
            Temp,
            Init,
            Load,
        }

        /// <summary>
        /// Тип используемой задачи
        /// </summary>
        public enum TypeTask {
            Test = 1,
            AddTask,
            SysTask
        }

        /// <summary>
        /// Тип задачи
        /// </summary>
        public readonly TypeTask Type;

        /// <summary>
        /// Название теста
        /// </summary>
        public readonly string NameTest;

        /// <summary>
        /// Индекс вкладки ControlTab первого уровня
        /// </summary>
        public readonly int? IndexTabL1;

        /// <summary>
        /// Индекс вкладки ControlTab вторго уровня
        /// </summary>
        public readonly int? IndexTabL2;

        /// <summary>
        /// Индекс страницы
        /// </summary>
        public readonly int? IndexPage;

        /// <summary>
        /// Название вкладки ControlTab первого уровня
        /// </summary>
        public readonly string NameTabL1;

        /// <summary>
        /// Название вкладки ControlTab второго уровня
        /// </summary>
        public readonly string NameTabL2;

        /// <summary>
        /// Текст кнопки
        /// </summary>
        public readonly string NameButton;

        /// <summary>
        /// Элемент привязки данных
        /// </summary>
        public TabControlViewModel Control { get; private set; }

        /// <summary>
        /// Максимальное занчение прогресс бара
        /// </summary>
        public readonly double MaxProgress;

        /// <summary>
        /// Указатель на функцию теста
        /// </summary>
        public readonly Func< int, int > Function;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameTest"></param>
        /// <param name="indexTabL1"></param>
        /// <param name="indexTabL2"></param>
        /// <param name="indexPage"></param>
        /// <param name="nameTabL1"></param>
        /// <param name="nameTabL2"></param>
        /// <param name="maxProgress"></param>
        /// <param name="kindTask"></param>
        /// <param name="func"></param>
        public ConfigTestsClass( TypeTask kindTask, string nameTest,
            int? indexTabL1, int? indexTabL2, int? indexPage, string nameTabL1, string nameTabL2,
            double maxProgress, Func< int, int > func ) {
            Type        = kindTask;
            NameTest    = nameTest;
            IndexTabL1  = indexTabL1;
            IndexTabL2  = indexTabL2;
            IndexPage   = indexPage;
            NameTabL1   = nameTabL1;
            NameTabL2   = nameTabL2;
            NameButton  = nameTest;
            Control     = null;
            MaxProgress = maxProgress;
            Function    = func;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public void SetControl( TabControlViewModel control ) {
            Control = control;
        }
    }
}