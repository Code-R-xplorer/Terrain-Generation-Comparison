using Mesh_Gen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Math;

public class MidpointDisplacement : MonoBehaviour
{
    public int MapWidth;
    public int MapDepth;

    int RecursionLevel = 0;
    //1, get the midpoint
    //2, divide it into quarters
    //3, divide it into sixteenths


    MeshGenerator MG;



    int BoxDepth;
    int BoxWidth;

    bool ReachedLimit = false;

    public float Roughness = 2;



    // Start is called before the first frame update
    void Start()
    {
        MG = GetComponent<MeshGenerator>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GenerateMidpointDisplacement()
    {
        //start by creating the shape
        MG.AltCreateShapeFunction(MapWidth, MapDepth);

        //it will stop the recursion if the boxes are of size 1. because there's no point in continuing then.
        ReachedLimit = false;

        RecursionLevel = 0;



        float Displacement = MapWidth * (1 / Mathf.Pow(2, Roughness));
        while (RecursionLevel < 1000)
        {
            BoxesFunction(RecursionLevel, Displacement);


            if (ReachedLimit)
            {
                RecursionLevel += 1000;
            }

            Displacement *= (1 / Mathf.Pow(2, Roughness));

            RecursionLevel++;



        }

        MG.MidpointUpdate();

    }


    void BoxesFunction(int R, float Dis)
    {
        BoxDepth = (MapDepth + 1) / Mathf.RoundToInt(Mathf.Pow(2, R));
        BoxWidth = (MapWidth + 1) / Mathf.RoundToInt(Mathf.Pow(2, R));


        if (BoxWidth * BoxDepth <= 1)
        {
            ReachedLimit = true;   
            return;
        }

        //if I had included the part for the right and bottom parts, this would've made sure that it's only done once for each
        bool RightOfMap = false;
        bool BottomOfMap = false;

        for (int i = 0; i < MG.GetVertices().Length - (MapWidth + 2);)
        {

            if (i + (BoxDepth * (MapWidth + 1)) >= MG.GetVertices().Length - (MapWidth + 2))
            {
                BottomOfMap = true;
            }

            for (int j = i; j < i + MapWidth;)
            {

                if (j + BoxWidth > i + MapWidth)
                {
                    RightOfMap = true;
                }

                GetCorners(j, BoxWidth, BoxDepth, Dis, RightOfMap, BottomOfMap);
                j += BoxWidth;
                

            }

            RightOfMap = false;



            i += BoxDepth * (MapWidth + 1);

        }



    }

    void GetCorners(int StartPoint, int BW, int BD, float DisplaceValue, bool OnTheRight, bool AtTheBottom)
    {
        float TotalCornerHeight = 0;
        Vector3[] VList = MG.GetVertices(); 
        
        int MidpointIndex = StartPoint + (BW * (BD / 2)) + (BW / 2);


        MidpointIndex = StartPoint + ((MapWidth + 1) * (BD / 2)) + (BW / 2) ;

        
        int CurrentIndex = StartPoint;
        int TopLeftCornerIndex = StartPoint % VList.Length;
        int TopRightCornerIndex = (StartPoint + BW) % VList.Length;

        int BottomLeftCornerIndex = (StartPoint + ((BW * (BD - 1)) - 1)) % VList.Length;
        int BottomRightCornerIndex = (StartPoint + ((BW * BD) - 1)) % VList.Length;


        
        TotalCornerHeight += VList[TopLeftCornerIndex].y + VList[TopRightCornerIndex].y +
            VList[BottomLeftCornerIndex].y + VList[BottomRightCornerIndex].y;

        TotalCornerHeight /= 4;

        if (MidpointIndex >= VList.Length)
        {
            return;
        }



        MG.SetVertexHeight(MidpointIndex, TotalCornerHeight + Random.Range(-DisplaceValue, DisplaceValue));

        //the square step


        int TopMidpointIndex = StartPoint + (BW / 2);
        int LeftMidpointIndex = MidpointIndex - (BW / 2);


        //top midpoint, subtract the MapWidth times half the height
        //left midpoint, subtract half the BoxWidth
        //right midpoint, add half the BoxWidth
        //bottom midpoint, same as the top but in reverse

        //then reuse the left midpoint as the top of the next one


        //for the top one, the bottom midpoint is the original midpoint
        //for the left one, the bottom midpoint is the original midpoint

        


        int TopLineMidpointIndex = (TopMidpointIndex - ((MapWidth + 1) * ProperIntDivision(BoxDepth, 2)));

        

        int LeftLineMidpointIndex = StartPoint;


        int RightLineMidpointIndex = StartPoint + BW;
        int BottomLineMidpointIndex = MidpointIndex;


        if (TopLineMidpointIndex <= 0)
        {
            TopLineMidpointIndex = VList.Length + TopLineMidpointIndex;
        }

        


        //if the remainder is greater than the previous one, that means it went to the next line
        if (LeftLineMidpointIndex % (MapWidth + 1) > MidpointIndex % (MapWidth + 1) || LeftLineMidpointIndex < 0)
        {
            //print(LeftLineMidpointIndex);
            LeftLineMidpointIndex += MapDepth + 1;
            //get the line the midpoint is on
            //then subtract it from the end of that
            //so if it was 40 by 40
            //39 - (the remainder)
            //it can never be more than the size of the map


            //add the 40 onto it
            //if it goes over, then subtract the mapwidth
            //but how on earth do you check that
            //i guess it'd be like
            //get all the multiples of mapwidth
            //and if it goes below the closest one to it then it's there

        }

        if (RightLineMidpointIndex % (MapWidth + 1) < MidpointIndex % (MapWidth + 1))
        {
            RightLineMidpointIndex -= MapDepth + 1;



        }


        float TotalDiamondHeight = VList[TopLineMidpointIndex].y + VList[LeftLineMidpointIndex].y + 
            VList[RightLineMidpointIndex].y + VList[BottomLineMidpointIndex].y;

        TotalDiamondHeight /= 4;


        MG.SetVertexHeight(TopMidpointIndex, TotalDiamondHeight + Random.Range(-DisplaceValue, DisplaceValue));
        
        //now for the left

        TopLineMidpointIndex = StartPoint;



        LeftLineMidpointIndex = LeftMidpointIndex - BoxWidth / 2;


        RightLineMidpointIndex = MidpointIndex;
        BottomLineMidpointIndex = BottomLeftCornerIndex;

        if (LeftLineMidpointIndex % (MapWidth + 1) > LeftMidpointIndex % (MapWidth + 1) || LeftLineMidpointIndex < 0)
        {
            LeftLineMidpointIndex += MapWidth + 1;
        }


        TotalDiamondHeight = VList[TopLineMidpointIndex].y + VList[LeftLineMidpointIndex].y +
        VList[RightLineMidpointIndex].y + VList[BottomLineMidpointIndex].y;

        TotalDiamondHeight /= 4;

        MG.SetVertexHeight(LeftMidpointIndex, TotalDiamondHeight + Random.Range(-DisplaceValue, DisplaceValue));

        

        if (OnTheRight && AtTheBottom)
        {
            //print("In the corner");
        }
        else if (OnTheRight)
        {
            //print("On the right!");
        }

        else if (AtTheBottom)
        {
            //print("At the bottom!");
        }


        //calculate for the top
        //then reuse for the left

        //reuse the bottom line from the top

        //it wraps around
    }


    void SetBottomLineHeight(int StartPoint, int BW, int BD)
    {

    }

    void SetRightLineHeight(int StartPoint, int BW, int BD)
    {

    }




    //if it goes out of the current row, then it should reappear on the current side
    //if it goes out of the column, then it can just be moved to the back of the list

    //since we always know what list it will be on, set the value to be the end of the line

    //have to use int for everything to do with the place in the list, if it had a decimal point it wouldn't work
    //but that caused some problems with division
    int ProperIntDivision(int Dividend, int Divisor)
    {

        return Mathf.FloorToInt(((float)Dividend / Divisor) + 0.5f);


    }


}
