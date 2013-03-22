namespace Colonies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Threading;

    using Colonies.Annotations;

    public sealed class WorldGridViewModel
    {
        private readonly Timer modelTimer;

        private int count = 0;

        private Cell[,] worldGrid;
        public Cell[,] WorldGrid
        {
            get
            {
                return this.worldGrid;
            }
            set
            {
                this.worldGrid = value;
            }
        }

        private List<List<Cell>> cells;
        public IEnumerable<IEnumerable<Cell>> Cells
        {
            get
            {
                if (this.cells == null)
                {
                    this.cells = new List<List<Cell>>();
                    for (var y = 0; y < this.WorldGrid.GetLength(1); y++)
                    {
                        var cellRow = new List<Cell>();

                        for (var x = 0; x < this.WorldGrid.GetLength(0); x++)
                        {
                            cellRow.Add(this.WorldGrid[x, y]);
                        }

                        this.cells.Add(cellRow);
                    } 
                }                

                return this.cells;
            }
        }

        private List<Occupant> occupants;
        public List<Occupant> Occupants
        {
            get
            {
                return this.occupants;
            }
            set
            {
                this.occupants = value;
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
            this.WorldGrid = new Cell[gridWidth, gridHeight];
            this.Occupants = new List<Occupant>();

            var random = new Random();

            for (var column = 0; column < gridWidth; column++)
            {
                for (var row = 0; row < gridHeight; row++)
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

                    this.WorldGrid[column, row] = new Cell(cellType, column, row);
                }
            }

            var occupant = new Occupant("testOccupant");
            this.WorldGrid[1, 1].Occupant = occupant;
        }

        private void UpdateModel(object state)
        {
            this.count++;

            //if (this.count == 50)
            //{
            //    this.WorldGrid[0, 0].Occupant = new Occupant("anotherOccupant");
            //}
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
