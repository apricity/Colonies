namespace Colonies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Colonies.Annotations;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int height;
        private int width;

        private List<List<Cell>> cells = new List<List<Cell>>();
        public List<List<Cell>> Cells
        {
            get
            {
                return this.cells;
            }
        }

        public MainWindowViewModel()
        {
            this.height = 4;
            this.width = 4;

            var random = new Random();

            for (var row = 0; row < this.height; row++)
            {
                var rowOfCells = new List<Cell>();

                for (var column = 0; column < this.width; column++)
                {
                    CellType cellType;

                    var randomNumber = random.Next(0, 4);
                    switch (randomNumber)
                    {
                        case 0:
                            cellType = CellType.Earth;
                            break;
                        case 1:
                            cellType = CellType.Grass;
                            break;
                        case 2:
                            cellType = CellType.Water;
                            break;
                        case 3:
                            cellType = CellType.Fire;
                            break;
                        default:
                            cellType = CellType.Unknown;
                            break;
                    }

                    rowOfCells.Add(new Cell(cellType));
                }

                this.cells.Add(rowOfCells);
            }
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
