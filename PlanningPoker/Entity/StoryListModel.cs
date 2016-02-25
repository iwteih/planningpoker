using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.Entity
{
    class StoryListModel : ITreeModel 
    {
        private ICollection<Story> storyList;
        public StoryListModel(ICollection<Story> list)
        {
            storyList = list;
        }

        public ICollection<Story> StoryList
        {
            get { return storyList; }
        }

        public System.Collections.IEnumerable GetChildren(object parent)
        {
            Story story = parent as Story;

            if(story == null)
            {
                return storyList.AsEnumerable();
            }

            return story.SubTasks.AsEnumerable();
        }

        public bool HasChildren(object parent)
        {
            return (parent as Story).SubTasks.Count > 0;
        }
    }
}
