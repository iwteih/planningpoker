using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.WCF
{
    /// <summary>
    /// Callback methods occur on client side
    /// </summary>
    public class Callback : ICallback
    {
        public event EventHandler<UserExitEventArgs> ExitEventHandler;
        /// <summary>
        /// Someone played a card.
        /// </summary>
        public event EventHandler PlayEventHandler;

        /// <summary>
        /// All the cards has been flipped.
        /// </summary>
        public event EventHandler FlipEventHandler;

        /// <summary>
        /// Server sends Reset command.
        /// </summary>
        public event EventHandler ResetEventHandler;

        /// <summary>
        /// Server wants to sync story.
        /// </summary>
        public event EventHandler<StorySyncArgs> StorySyncEventHandler;

        /// <summary>
        /// Server wants to sync story list.
        /// </summary>
        public event EventHandler<StoryListSyncArgs> StoryListSyncEventHandler;

        /// <summary>
        /// Server wants to sync story point.
        /// </summary>
        public event EventHandler<StorySyncArgs> StoryPointSyncEventHandler;

        private GameInfo gameInfo = GameInfo.Instance;

        public void Join(string moderator, string user, string role, Story story, string cardSequence, Participant[] participants)
        {
            lock (gameInfo)
            {
                List<Participant> toAppend = new List<Participant>();
                foreach (Participant remoteP in participants)
                {
                    bool exist = false;
                    foreach (Participant localP in gameInfo.ParticipantsList)
                    {
                        if (remoteP.ParticipantName == localP.ParticipantName)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        toAppend.Add(remoteP);
                    }
                }

                foreach (Participant toAdd in toAppend)
                {
                    gameInfo.ParticipantsList.Add(
                        new Participant()
                        {
                            ParticipantName = toAdd.ParticipantName,
                            Role = toAdd.Role,
                            PlayingCard = toAdd.PlayingCard,
                            UnflipedPlayingCard = toAdd.UnflipedPlayingCard
                        });
                }

                Participant p = gameInfo.ParticipantsList.FirstOrDefault(f => f.ParticipantName == user);

                if (p == null)
                {
                    p = new Participant()
                        {
                            ParticipantName = user,
                            Role = role,
                        };
                    p.Reset();
                    gameInfo.ParticipantsList.Add(p);
                }
                gameInfo.Moderator = moderator;

                if (gameInfo.CardSequenceString != cardSequence)
                {
                    gameInfo.LoadCardSequence(cardSequence);
                }

                if (StorySyncEventHandler != null)
                {
                    StorySyncEventHandler(null, new StorySyncArgs() { Story = story });
                }
            }
        }

        public void Play(string user, string pokerValue)
        {
            bool played = false;
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

                if (p != null)
                {
                    p.Play(pokerValue);
                    played = true;
                }
            }

            if (played)
            {
                if (PlayEventHandler != null)
                {
                    PlayEventHandler(null, null);
                }
            }
        }

        public void Withdraw(string user)
        {
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

                if (p != null)
                {
                    p.Reset();
                }
            }
        }

        public void Exit(string user)
        {
            bool exit = false;
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

                if (p != null)
                {
                    gameInfo.ParticipantsList.Remove(p);
                    exit = true;
                }
            }

            if (exit && ExitEventHandler != null)
            {
                ExitEventHandler(null, new UserExitEventArgs() { ExitUser = user });
            }
        }

        public void Flip()
        {
            lock (gameInfo)
            {
                foreach (Participant p in gameInfo.ParticipantsList)
                {
                    p.Flip();
                }
            }

            if (FlipEventHandler != null)
            {
                FlipEventHandler(null, null);
            }
        }

        public void Reset()
        {
            lock (gameInfo)
            {
                foreach (Participant p in gameInfo.ParticipantsList)
                {
                    p.Reset();
                }
            }

            if (ResetEventHandler != null)
            {
                ResetEventHandler(null, null);
            }
        }

        public void ShowScore(string score)
        {
            gameInfo.Score = score;
        }

        public void SyncStory(Story story)
        {
            gameInfo.CurrentStory = story;

            if (StorySyncEventHandler != null)
            {
                StorySyncEventHandler(null, new StorySyncArgs() { Story = story });
            }
        }

        public void SyncStoryList(List<Story> storyList)
        {
            if (StoryListSyncEventHandler != null)
            {
                StoryListSyncEventHandler(null, new StoryListSyncArgs() { StoryList = storyList });
            }
        }

        public void SyncStoryPoint(Story story)
        {
            if (StoryPointSyncEventHandler != null)
            {
                StoryPointSyncEventHandler(null, new StorySyncArgs() { Story = story });
            }
        }
    }
}
