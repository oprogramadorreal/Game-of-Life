using System.Collections.Generic;
using UnityEngine;

public sealed class DummyBoardObject : BoardObject
{
    [SerializeField]
    private List<Transform> poses;

    private readonly List<Transform> currentPoseParts = new List<Transform>();

    public override IEnumerable<MetaCell> GetCellsOn(IBoard board)
    {
        foreach (var t in currentPoseParts)
        {
            var cellIndex = board.PickCell(t.position);

            yield return new MetaCell()
            {
                Index = cellIndex,
                Type = BoardCellType.Alive
            };
        }
    }

    private void Start()
    {
        LoadPose(0);
    }

    private void LoadPose(int poseIndex)
    {
        if (poseIndex < 0 || poseIndex >= poses.Count)
        {
            return;
        }

        currentPoseParts.Clear();

        var currentPose = poses[poseIndex];

        for (var i = 0; i < currentPose.childCount; ++i)
        {
            currentPoseParts.Add(currentPose.GetChild(i).transform);
        }
    }
}
