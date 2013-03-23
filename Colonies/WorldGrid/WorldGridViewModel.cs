namespace Colonies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Threading;

    using Colonies.Annotations;

    public sealed class WorldGridViewModel : INotifyPropertyChanged
    {
        private readonly Timer modelTimer;

        private int count = 0;

        private List<List<Cell>> cells;
        public List<List<Cell>> Cells
        {
            get
            {
                return this.cells;
            }
            set
            {
                this.cells = value;
                this.OnPropertyChanged("Cells");
            }
        }

        private List<Occupant> Occupants { get; set; }

        private List<Cell> ListOfOccupiedCells
        {
            get
            {
                var result = new List<Cell>();
                foreach (var cellRow in this.Cells)
                {
                    foreach (var cell in cellRow)
                    {
                        if (cell.HasOccupant)
                        {
                            result.Add(cell);
                        }
                    } 
                }

                return result;
            }
        }

        public WorldGridViewModel()
        {
            this.InitialiseModel(5, 5);

            TimerCallback modelTimerCallback = this.UpdateModel;
            this.modelTimer = new Timer(modelTimerCallback, null, 0, 250);
        }

        private void InitialiseModel(int gridHeight, int gridWidth)
        {
            this.Cells = new List<List<Cell>>();
            this.Occupants = new List<Occupant>();

            var random = new Random();

            for (var row = 0; row < gridHeight; row++)
            {
                var cellRow = new List<Cell>();
                for (var column = 0; column < gridWidth; column++)
                {
                    CellType cellType;

                    var randomNumber = random.Next(0, 4);
                    switch (column)
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

                    cellRow.Add(new Cell(cellType, column, row));
                }

                this.Cells.Add(cellRow);
            }

            var occupant = new Occupant("testOccupant");
            this.Cells[1][1].Occupant = occupant;
        }

        private void UpdateModel(object state)
        {
            this.count++;

            if (this.count == 20)
            {
                foreach (var cell in ListOfOccupiedCells)
                {
                    var cellOccupant = cell.Occupant;
                    cell.Occupant = null;
                    this.Cells[cell.X + 1][cell.Y].Occupant = cellOccupant;
                }

                this.Cells[0][0].Occupant = new Occupant("anotherOccupant");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
