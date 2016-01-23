using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PlanningPoker.Entity
{
    public class Participant : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private string participantName;
        public string ParticipantName
        {
            get { return participantName; }
            set
            {
                if (participantName != value)
                {
                    participantName = value;
                    OnPropertyChanged("ParticipantName");
                }
            }
        }

        private string playingCard;
        public string PlayingCard
        {
            get { return playingCard; }
            set
            {
                if (playingCard != value)
                {
                    playingCard = value;
                    OnPropertyChanged("PlayingCard");
                }
            }
        }

        private string role;
        public string Role
        {
            get { return role; }
            set
            {
                if(role != value)
                {
                    role = value;
                    OnPropertyChanged("Role");
                }
            }
        }
    }
}
