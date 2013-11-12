namespace Wacton.Colonies.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;

    public class EcosystemData
    {
        // TODO: these should be private, accessible through methods only
        public Habitat[,] Habitats { get; private set; }
        public Dictionary<Organism, Habitat> OrganismHabitats { get; private set; }
        public Dictionary<Habitat, Coordinate> HabitatCoordinates { get; private set; }
        public Dictionary<Coordinate, List<Measure>> CoordinateHazards { get; set; }

        public int Width
        {
            get
            {
                return this.Habitats.Width();
            }
        }

        public int Height
        {
            get
            {
                return this.Habitats.Height();
            }
        }

        public EcosystemData(Habitat[,] habitats, Dictionary<Organism, Habitat> organismHabitats)
        {
            this.Habitats = habitats;
            this.OrganismHabitats = organismHabitats;
            this.HabitatCoordinates = new Dictionary<Habitat, Coordinate>();
            this.CoordinateHazards = new Dictionary<Coordinate, List<Measure>>();

            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatCoordinates.Add(this.Habitats[i, j], new Coordinate(i, j));
                }
            }
        }

        // TODO: use interface?
        public List<Organism> GetOrganisms()
        {
            return this.OrganismHabitats.Keys.ToList();
        }

        public Dictionary<Organism, Coordinate> GetOrganismCoordinates()
        {
            return this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key,
                organismHabitat => this.HabitatCoordinates[organismHabitat.Value]);
        }

        public Habitat HabitatAt(Coordinate coordinate)
        {
            return this.Habitats[coordinate.X, coordinate.Y];
        }

        public Habitat HabitatOf(Organism organism)
        {
            return this.OrganismHabitats[organism];
        }

        public Coordinate CoordinateOf(Habitat habitat)
        {
            return this.HabitatCoordinates[habitat];
        }

        public Coordinate CoordinateOf(Organism organism)
        {
            return this.HabitatCoordinates[this.OrganismHabitats[organism]];
        }
    }
}