using Mesh_Gen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Math;

public class MidpointDisplacement : MonoBehaviour
{
    public int MapWidth;
    public int MapDepth;


    //int TopLeftCornerIndex;
    //int TopRightCornerIndex;
    //int BottomLeftCornerIndex;
    //int BottomRightCornerIndex;



    int RecursionLevel = 0;
    //1, get the midpoint
    //2, divide it into quarters
    //3, divide it into sixteenths


    MeshGenerator MG;

    int MaxRecursion;


    int BoxDepth;
    int BoxWidth;

    bool RunOnce = false;
    bool ReachedLimit = false;

    public float Roughness = 2;



    // Start is called before the first frame update
    void Start()
    {
        MG = GetComponent<MeshGenerator>();
        MG.AltCreateShapeFunction(MapWidth, MapDepth);

    }

    // Update is called once per frame
    void Update()
    {
        //if (!RunOnce)
        //{
        //    RunOnce = true;
        //    GenerateMidpointDisplacement();
        //}
    }

    public void GenerateMidpointDisplacement()
    {
        ReachedLimit = false;

        RecursionLevel = 0;



        float Displacement = MapWidth * (1 / Mathf.Pow(2, Roughness));

        //while (RecursionLevel < 5)
        while (RecursionLevel < 100)
        {

            BoxesFunction(RecursionLevel, Displacement);


            if (ReachedLimit)
            {
                RecursionLevel = 100;
            }

            Displacement *= (1 / Mathf.Pow(2, Roughness));

            //print(Displacement);
            RecursionLevel++;



        }
        




        //BoxesFunction(1);
        //BoxesFunction(2);

        //MG.SetVertexHeight(30, -1000);
        //MG.SetVertexHeight(60, 1000);
        //MG.SetVertexHeight(120, -10000);
        //MG.SetVertexHeight(240, 1000000);

        MG.MidpointUpdate();


        //get the value it's being divided by in each instance, and then work it out from that
        //2 should be done in one step
        //


        


        //get the four corners for that sector
        //if (RecursionLevel == 0)
        //{
        //    TotalCornerHeight += MG.GetVertices()[0].y;
        //    MG.SetVertexHeight(MG.GetVertices().Length / 2, Random.Range(-10, 10));
        //}

    }


    void BoxesFunction(int R, float Dis)
    {
        BoxDepth = (MapDepth + 1) / Mathf.RoundToInt(Mathf.Pow(2, R));
        BoxWidth = (MapWidth + 1) / Mathf.RoundToInt(Mathf.Pow(2, R));

        //print(BoxDepth);
        //print(BoxWidth);
        //print(BoxWidth * BoxDepth);

        if (BoxWidth * BoxDepth <= 1)
        {
            print(R);
            ReachedLimit = true;   
            return;
        }


        //print(BoxDepth);
        //print(BoxWidth);
        //print(MG.GetVertices().Length);

        //print(MapWidth * (MapDepth / 2));
        //print(MG.GetVertices().Length - (MapWidth + 2));

        bool RightOfMap = false;
        bool BottomOfMap = false;

        for (int i = 0; i < MG.GetVertices().Length - (MapWidth + 2);)
        {

            if (i + (BoxDepth * (MapWidth + 1)) >= MG.GetVertices().Length - (MapWidth + 2))
            {
                BottomOfMap = true;
            }

            for (int j = i; j < i + MapWidth;)
            //for (int j = 0; i < MapWidth;)
            {
                //print(j);
                //j += 100;
                //print("J is currently " + j);

                if (j + BoxWidth > i + MapWidth)
                {
                    RightOfMap = true;
                }

                GetCorners(j, BoxWidth, BoxDepth, Dis, RightOfMap, BottomOfMap);
                j += BoxWidth;
                

            }

            RightOfMap = false;

            //GetCorners(i, BoxWidth, BoxDepth);
            //print(i);
            //print("i is " + i);
            //print(MapWidth + 1);
            //print(BoxDepth);


            i += BoxDepth * (MapWidth + 1);

            //print("I has become " + i);
        }


        //MG.SetVertexHeight(820, 4000);

        //GetCorners(0, MapWidth, MapDepth);

    }

    void GetCorners(int StartPoint, int BW, int BD, float DisplaceValue, bool OnTheRight, bool AtTheBottom)
    {
        float TotalCornerHeight = 0;
        Vector3[] VList = MG.GetVertices(); 
        
        int MidpointIndex = StartPoint + (BW * (BD / 2)) + (BW / 2);


        MidpointIndex = StartPoint + ((MapWidth + 1) * (BD / 2)) + (BW / 2) ;

        //print(MidpointIndex);

        //print(MidpointIndex);
        //print(MG.GetVertices().Length / 2);
        //Find the midpoint

        int CurrentIndex = StartPoint;
        int TopLeftCornerIndex = StartPoint % VList.Length;
        int TopRightCornerIndex = (StartPoint + BW) % VList.Length;

        int BottomLeftCornerIndex = (StartPoint + ((BW * (BD - 1)) - 1)) % VList.Length;
        int BottomRightCornerIndex = (StartPoint + ((BW * BD) - 1)) % VList.Length;


        //BottomLeftCornerIndex = BottomLeftCornerIndex % VList.Length;
        //BottomRightCornerIndex = BottomRightCornerIndex % VList.Length;

        //print(BottomRightCornerIndex);

        //print("The left" + " " + TopLeftCornerIndex + " " + TopRightCornerIndex);
        //print("The h" + + BottomLeftCornerIndex + " " + BottomRightCornerIndex);
        
        
        TotalCornerHeight += VList[TopLeftCornerIndex].y + VList[TopRightCornerIndex].y +
            VList[BottomLeftCornerIndex].y + VList[BottomRightCornerIndex].y;

        TotalCornerHeight /= 4;

        //print(TotalCornerHeight);

        //if (MidpointIndex < 961)
        //{
        //    //MG.SetVertexHeight(TopLeftCornerIndex, 20);
        //    //MG.SetVertexHeight(TopRightCornerIndex, 20);
        //    //MG.SetVertexHeight(BottomLeftCornerIndex, 20);
        //    //MG.SetVertexHeight(BottomRightCornerIndex, 20);
        //}

        //float DisplaceValue = Mathf.Clamp((MapWidth * MapDepth) / (Mathf.Pow(2, RecursionLevel + 1)), 0, 20);
        //float DisplaceValue = ((MapWidth * MapDepth) / (Mathf.Pow(2, RecursionLevel + 1))) / 20;
        //float DisplaceValue = ((MapWidth * MapDepth) / (Mathf.Pow(2, RecursionLevel + 1)));
        //float DisplaceValue = MapDepth / Mathf.Pow(2, RecursionLevel + 1);




        


        //DisplaceValue /= 10;

        ////if (DisplaceValue > 0.5f)
        ////{
        ////    DisplaceValue = 0.5f;
        ////}

        //print(DisplaceValue);


        if (MidpointIndex >= VList.Length)
        {
            return;
        }



        if (TotalCornerHeight != 0)
        {
            //print(TotalCornerHeight);
        }
        MG.SetVertexHeight(MidpointIndex, TotalCornerHeight + Random.Range(-DisplaceValue, DisplaceValue));
        //MG.SetVertexHeight(MidpointIndex, TotalCornerHeight + Random.Range(0, DisplaceValue));

        //the square step


        int TopMidpointIndex = StartPoint + (BW / 2);
        int LeftMidpointIndex = MidpointIndex - (BW / 2);


        //top midpoint, subtract the MapWidth times half the height
        //left midpoint, subtract half the BoxWidth
        //right midpoint, add half the BoxWidth
        //bottom midpoint, same as the top but in reverse

        //then reuse the left midpoint as the top of the next one

        //don't bother calculating for the bottom two, they'll be gotten to eventually

        //for the top one, the bottom midpoint is the original midpoint
        //for the left one, the bottom midpoint is the original midpoint

        //


        int TopLineMidpointIndex = (TopMidpointIndex - ((MapWidth + 1) * ProperIntDivision(BoxDepth, 2)));

        //print(BoxDepth / 2);
        //print(Mathf.Ceil((float)BoxDepth / 2f));



        //int TopLineMidpointIndex = (StartPoint + (BoxDepth / 2)) % VList.Length;
        int LeftLineMidpointIndex = StartPoint;
        //int LeftLineMidpointIndex = (TopMidpointIndex - ProperIntDivision(BoxWidth, 2));


        int RightLineMidpointIndex = StartPoint + BW;
        //int RightLineMidpointIndex = TopMidpointIndex + ProperIntDivision(BoxWidth, 2);
        int BottomLineMidpointIndex = MidpointIndex;


        if (TopLineMidpointIndex <= 0)
        {
            //print(TopLineMidpointIndex);
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



            //if it goes less then how on earth do you check
        }

        if (RightLineMidpointIndex % (MapWidth + 1) < MidpointIndex % (MapWidth + 1))
        {
            RightLineMidpointIndex -= MapDepth + 1;


            //if (RightLineMidpointIndex >= VList.Length)
            //{
            //    RightLineMidpointIndex = VList.Length - 1;
            //}

        }

        //int LeftLineMidpointIndex = StartPoint;


        //print(MidpointIndex);


        //print(BW);
        //print(VList.Length);
        //print(TopLineMidpointIndex);
        //print(LeftLineMidpointIndex);
        //print(BottomLineMidpointIndex);
        //print(RightLineMidpointIndex);



        float TotalDiamondHeight = VList[TopLineMidpointIndex].y + VList[LeftLineMidpointIndex].y + 
            VList[RightLineMidpointIndex].y + VList[BottomLineMidpointIndex].y;

        TotalDiamondHeight /= 4;


        MG.SetVertexHeight(TopMidpointIndex, TotalDiamondHeight + Random.Range(-DisplaceValue, DisplaceValue));
        //MG.SetVertexHeight(TopLineMidpointIndex, 200);
        //MG.SetVertexHeight(LeftLineMidpointIndex, 200);
        //MG.SetVertexHeight(RightLineMidpointIndex, 200);
        //MG.SetVertexHeight(BottomLineMidpointIndex, -200);

        //now for the left

        TopLineMidpointIndex = StartPoint;

        //print(BoxDepth / 2);
        //print(Mathf.Ceil((float)BoxDepth / 2f));



        //int TopLineMidpointIndex = (StartPoint + (BoxDepth / 2)) % VList.Length;
        LeftLineMidpointIndex = LeftMidpointIndex - BoxWidth / 2;
        //int LeftLineMidpointIndex = (TopMidpointIndex - ProperIntDivision(BoxWidth, 2));


        RightLineMidpointIndex = MidpointIndex;
        //int RightLineMidpointIndex = TopMidpointIndex + ProperIntDivision(BoxWidth, 2);
        BottomLineMidpointIndex = BottomLeftCornerIndex;

        if (LeftLineMidpointIndex % (MapWidth + 1) > LeftMidpointIndex % (MapWidth + 1) || LeftLineMidpointIndex < 0)
        {
            LeftLineMidpointIndex += MapWidth + 1;
        }


        //print(LeftMidpointIndex);
        //print("Top line is " + TopLineMidpointIndex);
        //print("Left line is " + LeftLineMidpointIndex);
        //print("Right line is " + RightLineMidpointIndex);
        //print("Bottom line is " + BottomLineMidpointIndex);


        TotalDiamondHeight = VList[TopLineMidpointIndex].y + VList[LeftLineMidpointIndex].y +
        VList[RightLineMidpointIndex].y + VList[BottomLineMidpointIndex].y;

        TotalDiamondHeight /= 4;

        MG.SetVertexHeight(LeftMidpointIndex, TotalDiamondHeight + Random.Range(-DisplaceValue, DisplaceValue));

        //MG.SetVertexHeight(MidpointIndex, 200);


        if (OnTheRight && AtTheBottom)
        {
            print("In the corner");
        }
        else if (OnTheRight)
        {
            print("On the right!");
        }

        else if (AtTheBottom)
        {
            print("At the bottom!");
        }


        //calculate for the top
        //then reuse for the left

        //reuse the bottom line from the top

        //it wraps around






        //MG.SetVertexHeight(MidpointIndex, 200);

        //print(TotalCornerHeight);



        //look at where it is
        //needs to work out when it will stop
        //when it doesn't have any more vertices to select?

        //this just gets the corners for a certain quadrant
        //pass in what it's being divided by
        //and which section it is
        //so if divided by 1, then get the midpoint overall

        //divided by four, then get each section of that
        //just go until it stops working to see where you should be stopping


        //then get the GUI and the report started
    }


    void SetBottomLineHeight(int StartPoint, int BW, int BD)
    {

    }

    void SetRightLineHeight(int StartPoint, int BW, int BD)
    {

    }


    //int CustomModFunction()
    //{
    //    //using % in C# gives the reminder
    //    //and not the modulo like any reasonable human would assume it does
    //    //so when I want to use the symbol for modulo to get the modulo I have to write it myself

    //    //only needs to be used when subtracting, since that's the only case where it can go to a negative index





    //    return 0;
    //}



    //if it goes out of the current row, then it should reappear on the current side
    //if it goes out of the column, then it can just be moved to the back of the list

    //since we always know what list it will be on, set the value to be the end of the line

    //have to use int for everything to do with the place in the list, if it had a decimal point it wouldn't work
    //but that caused some problems with division
    //I got tired of writing this out every time so I just added this
    int ProperIntDivision(int Dividend, int Divisor)
    {
        //print((float)Dividend / Divisor);
        //print(Mathf.Round(10.5f));
        //print(Mathf.FloorToInt(10.6f + 0.5f));
        //print(Mathf.FloorToInt(((float)Dividend / Divisor) + 0.5f));


        //print(Mathf.Round((float)Dividend / Divisor));
        return Mathf.FloorToInt(((float)Dividend / Divisor) + 0.5f);


    }


}
