using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public class Ecosystem : INotifyPropertyChanged
    {
        private List<List<Niche>> niches;
        public List<List<Niche>> Niches
        {
            get
            {
                return this.niches;
            }
            set
            {
                this.niches = value;
                this.OnPropertyChanged("Niches");
            }
        }

        public int Height { get; private set; }
        public int Width { get; private set; }

        public Ecosystem()
        {
            this.Height = Properties.Settings.Default.BoardHeight;
            this.Width = Properties.Settings.Default.BoardWidth;

            var board = new List<List<Niche>>(this.Height);

            for (int i = 0; i < board.Capacity; i++)
            {
                board.Add(new List<Niche>(this.Width));

                for (int j = 0; j < board[i].Capacity; j++)
                {
                    board[i].Add(new Niche(null, null));
                }
            }

            this.niches = board;
        }

        public void SetNiche(int x, int y, Niche niche)
        {
            this.niches[x][y] = niche;
        }

        public Niche GetNiche(int x, int y)
        {
            return this.niches[x][y];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
