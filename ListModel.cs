using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using NUnit.Framework.Internal.Commands;

/*
    Я попробовал максимально избавиться от параметров в функциях и построить всю логику на конструкторах, так
    как я где-то читал, что паттерн Команда лучше реализовывать на конструкторах
    (Ну и еще мне нужно было просто повторить тему конструкторов).
    
    Вообще в этой задаче было бы гораздо проще не использовать этот паттерн, но я решил попробовать свои силы.
    
    Само собой, я тут много где накосячил. Это был просто эксперимент для меня.
    Схема была такая:
    
    1) В методах ListModel<TItem> создается по экземпляру соответствующего класса команды (в параметрах конструкторах
    указываются все необходимые данные для конкретного случая)
    2) Затем в Command создается экземпляр класса Receiver(где и содержится вся необходимая нам логика) с необходимыми
    конкретного случая данными.
    3) уже в Reciever происходят все необходимые нам операции по сути все с теми же данными.
    
    *ВОПРОС* При вот таких пересылках создаются копии данных или лишь ссылки на один и тот же участок памяти?
*/
namespace TodoApplication
{
    public interface ICommand<T>
    {
        void Do();
        void Undo();
    }

    public class Receiver <T>
    {
        private T item;
        private int index;
        private List<T> items;
        public Receiver(List<T> items,T item)
        {
            this.items = items;
            this.item = item;
        }

        public Receiver(int index, List<T> list)
        {
            this.index = index;
            items = list;
        }

        public void Add() => items.Add(item);

            public void Remove()
        {
            item = items[index]; //вот эта строчка самая важная! 
            items.RemoveAt(index);
        }

        public void UndoAfterRemove() => items.Insert(index, item);
        
        public void UndoAfterAdd() => items.Remove(item);
    }

    public class CommandAdd<T> : ICommand<T>
    {
        public Receiver<T> receiver;

        public CommandAdd(List<T> items,T item)
        {
            receiver = new Receiver<T>(items, item);
        }
        public void Do() => receiver.Add();
        
        public void Undo() => receiver.UndoAfterAdd();
    }

    public class CommandRemove<T> : ICommand<T>
    {
        private int index;
        private Receiver<T> _receiver;
        public CommandRemove(int index, List<T> items)
        {
            _receiver = new Receiver<T>(index, items);
        }

        public void Do() => _receiver.Remove();
        

        public void Undo() => _receiver.UndoAfterRemove();
        
    }
    
    public class ListModel<TItem>
    {
        public List<TItem> Items { get; }
        public int Limit;
        public LimitedSizeStack<ICommand<TItem>> SizeStack;
        public ListModel(int limit)
        {
            Items = new List<TItem>();
            Limit = limit;
            SizeStack = new LimitedSizeStack<ICommand<TItem>>(Limit);
        }

        public void AddItem(TItem item)
        {
          var commandAdd = new CommandAdd<TItem>(Items, item);
          SizeStack.Push(commandAdd);
          commandAdd.Do();
        }

        public void RemoveItem(int index)
        {
           var commandRemove = new CommandRemove<TItem>(index, Items);
           SizeStack.Push(commandRemove);
           commandRemove.Do();
        }

        public bool CanUndo() => SizeStack.Count > 0;

        public void Undo()
        {
            if (CanUndo())
            {
                SizeStack.Pop().Undo();
            }
        }
    }
}
