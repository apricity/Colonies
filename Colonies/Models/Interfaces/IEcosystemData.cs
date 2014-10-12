namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;

    public interface IEcosystemData
    {
        int Width { get; }

        int Height { get; }

        IEnumerable<Coordinate> AllCoordinates();

        bool HasLevel(Coordinate coordinate, EnvironmentMeasure measure);

        bool HasLevel(Coordinate coordinate, OrganismMeasure measure);

        double GetLevel(Coordinate coordinate, EnvironmentMeasure measure);

        double GetLevel(Coordinate coordinate, OrganismMeasure measure);

        IOrganism GetOrganism(Coordinate coordinate);

        IEnvironment GetEnvironment(Coordinate coordinate);

        void InsertHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate);

        void RemoveHazardSource(EnvironmentMeasure environmentMeasure, Coordinate coordinate);

        IEnumerable<Coordinate> GetHazardSourceCoordinates(EnvironmentMeasure hazardMeasure);
    }
}
