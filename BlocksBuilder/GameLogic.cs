using System;

namespace BlocksBuilder
{
    class GameLogic
    {
        public delegate void ShouldBeCutHandler(int cutThisLength, bool normalFromTheLeftSide); 
        public event ShouldBeCutHandler CutItDude;
        public event Action GameOver;

        public int Direction { get; private set; }

        public int BlockHeight { get; private set; }
        public int EndOfField { get; private set; }
        public int DownOfField { get; private set; }

        public int CurrentSize { get; private set; }
        public int LeftBorder { get; private set; }
        public int RightBorder { get; private set; }
        public int CurrentBlockWidth
        {
            get
            {
                return RightBorder - LeftBorder;
            }
        }
        public int CurrentBlockLevel
        {
            get
            {
                return DownOfField - (CurrentSize+1) * BlockHeight;
            }
        }
        public int CurrentBlockCoordinates { get; private set; }

        public GameLogic()
        {
            CurrentBlockCoordinates = 0;
            Direction = 5;
            CurrentSize = 0; 
            LeftBorder = 0;
            RightBorder = 300;
            BlockHeight = 40;
            EndOfField = 660;
            DownOfField = 660;
        }

        public void DoMove()
        {
            if ((CurrentBlockCoordinates < 0) ||
                (CurrentBlockCoordinates + CurrentBlockWidth > EndOfField))
            {
                Direction *= -1;
            }

            CurrentBlockCoordinates += Direction;
        }

        public bool GetUpdate(int leftX)
        {
            // this method is called when player "throws" a block
            CurrentSize++;
            if (CurrentSize == 1)
            {
                LeftBorder = leftX;
                RightBorder += leftX;
                return true;
            }
            if ((LeftBorder > leftX + CurrentBlockWidth)|| RightBorder < leftX)
            {
                GameOver();
                return false;
            }
            if (leftX >= LeftBorder)
            { 
                CutItDude(leftX + CurrentBlockWidth - RightBorder , true);
                LeftBorder = leftX;
                if (CurrentBlockWidth == 0)
                {
                    GameOver(); return false;
                }
                return true;
            } else
            {           
                CutItDude(LeftBorder - leftX, false);
                RightBorder = leftX + CurrentBlockWidth;
                if (CurrentBlockWidth == 0)
                {
                    GameOver(); return false;
                }
                return true;
            }
        }

        public void RaiseSpeed()
        {
            Direction += (4 * Math.Sign(Direction));
        }
        public void ResetSpeed()
        {
            Direction = 5;
        }

        public void CleanAll()
        {
            CurrentBlockCoordinates = 0;
            CurrentSize = 0;
            LeftBorder = 0;
            RightBorder = 300;
            BlockHeight = 40;
            EndOfField = 660;
            DownOfField = 660;
        }
    }
}
