using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PlanningPoker.Entity
{
    enum Role
    {
        Dev,
        QA,
        PM,
        Moderator
    }

    [DataContract]
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
        [DataMember]
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
        [DataMember]
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

        [DataMember]
        public string UnflipedPlayingCard
        {
            get;
            set;
        }

        private string role;
        [DataMember]
        public string Role
        {
            get { return role; }
            set
            {
                if (role != value)
                {
                    role = value;
                    OnPropertyChanged("Role");
                }
            }
        }

        public void Play(String pokerValue)
        {
            PlayingCard = CardStatus.Ready.ToString();
            UnflipedPlayingCard = pokerValue;
        }

        public void Flip()
        {
            PlayingCard = UnflipedPlayingCard;
        }

        public void Reset()
        {
            PlayingCard = CardStatus.Pending.ToString();
            UnflipedPlayingCard = "-";
        }
    }

}
