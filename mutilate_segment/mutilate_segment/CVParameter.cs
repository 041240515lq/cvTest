using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mutilate_segment
{
   
   public class CVParameter
    {

         //???????
          public float WE2Mode;
          public float WE2ConstantPotential ;
          public float WE2DifferencePotential ;
          public float WE2Sensitivity ;
          public float SwapWE2WE1 ;
          public float PotentialFilter ;
          public float WE1ConvFilter ;
          public float WE1SingleFilter ;
          public float WE2ConvFilter ;
          public float WE2SingleFilter ;
          //???TXT ???
		  public float InitialPotential ;
		  public float FirstVertexPotential ;
		  public float SecondVertexPotential ;
		  public float FinalPotential ;
		  public float ScanRate ; 
        public float SweepSegments ;
		  public float SampleInterval ;
		  public float QuietTime ;
		  public float Sensitivity ;
		  public float EnableTerminatePotential ;
   
    } 
}
