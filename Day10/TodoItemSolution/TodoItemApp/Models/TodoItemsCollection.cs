using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace TodoItemApp.Models
{
    public class TodoItemsCollection : ObservableCollection<TodoItem> // IList<TodoItem>, List<TodoItem> 
    {
        public void CopyFrom(IEnumerable<TodoItem> todoItems)
        {
            this.Items.Clear(); // this.Items == TodoItemsCollection // ObservableCollection<리스트> 자체가 Items 속성을 가지고 있음. 모든데이터 삭제

            foreach (TodoItem item in todoItems) // todoItems == List 
            {
                this.Items.Add(item); // 하나씩 다시 추가
            }

            // 데이터 바꼈다고 알리고 리셋(전부 초기화)
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
