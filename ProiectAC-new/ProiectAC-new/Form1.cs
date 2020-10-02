using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace ProiectAC_new
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string[] toate_liniile = System.IO.File.ReadAllLines(@"D:\\AC\\nou\\ProiectAC-new\\fisier.asm");
        int index_linie = 0, index_et = 0, index_IR = 0, nrInstr = 0;
        string[] Mcuv;

        string LINIE = null;
      //  int INDEX_GLOBAL = 0;

        string Rs, Rd, Adresa, instructiune;
        
        string IR, MIR;
        int MAR=0, ACLOW = 0, ILLEGAL=0;
        int A = 0, M = 0; 
        int[] R = new int[16];
        int[] Mm = { 10, 20, 30, 40, 50 };
        int IFCH=0, READ=0, WRITE=0;
        int SBUS, DBUS, ALU, RBUS, MBUS, SP, IVR, FLAGS, ADR;
        int BVI=0, C, S, Z, V, BE0=0, BE1=0, Bi=0, BE=0, Cout, Sr, Zr,Vr, PC=0;
        int micro_index;
        int INDEX_RG;
        int CL0, CL1;
        //int to bin
        string int_to_bin(int nrInstr, int nr_total_biti)//binar pe 4 biti
        {
            
            int val;
            string a = "";

           if(nrInstr == 0 )
                a += (0).ToString();

            while (nrInstr >= 1)
            {
                val = nrInstr / 2;
                a += (nrInstr % 2).ToString();
                nrInstr = val;
            }

            string binValue = "";

            for (int i = a.Length - 1; i >= 0; i--)
            {
                binValue = binValue + a[i];
            }
            
            binValue = binValue.PadLeft(nr_total_biti, '0'); // lung totala si ce adaug
            return binValue;
        }
        
        //pentru etihete
        int index_linie2 = 0, PC2 = 0, index_et2 = 0;
        void iau_etichete()
        {
            string[] toate_liniile2 = System.IO.File.ReadAllLines(@"D:\\AC\\nou\\ProiectAC-new\\fisier.asm");
            
            int lungime2 = toate_liniile2.Length;
            string linie2;

            while (PC2 / 4 < lungime2)
            {
                index_linie2 = PC2 / 4;
                linie2 = toate_liniile2[index_linie2];
                string[] cuvSeparat2 = linie2.Split(' '); // baga in vector cuvintele separate
                int ct2 = 0;
                char ultimul_caracter2 = cuvSeparat2[ct2][cuvSeparat2[ct2].Length - 1]; //vf daca am eticheta ( daca se termina cu : )
          
                if (ultimul_caracter2 == ':')// am eticheta:_
                {
                    Etichete.Text += cuvSeparat2[ct2] + " " + PC2.ToString() + " " + "\n";
                    index_et2++; ct2++;
                }
                PC2 += 4;
            }
        }

        //reverse pt ca am stringuri
        string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        //getClasa---------------------------------------------------------------------------
        int getClasa(string IR)
        {
            if (IR[15] == '0') { index1 = 1;  return 1; }
            else if (IR[15] == '1' && IR[14] == '0' && IR[13] == '1') { index1 = 3; return 3; }
            else if (IR[15] == '1' && IR[14] == '1') { index1=4; return 4; }
            else if (IR[15] == '1' && IR[14] == '0') { index1= 2; return 2;  }
            else return -1;
        }
        
        //decodificare opcodeului-------------------------------------------------------------
        int Dec_Opcode(string instr)
        {
            //pentru prima clasa
 
            if (instr == "mov" || instr == "add" || instr == "sub" ||
                  instr == "cmp" || instr == "and" || instr == "or" ||
                      instr == "xor")
            {
                if (instr == "mov") return 0;
                if (instr == "add") return 1;
                if (instr == "sub") return 2;
                if (instr == "cmp") return 3;
                if (instr == "and") return 4;
                if (instr == "or") return 5;
                if (instr == "xor") return 6;
            }
            else //clasa a 2a
            {
                if (instr == "clr" || instr == "neg" || instr == "inc" ||
                    instr == "dec" || instr == "asl" || instr == "asr" ||
                    instr == "lsr" || instr == "rol" || instr == "ror" ||
                    instr == "rlc" || instr == "rrc" || instr == "jmp" ||
                    instr == "call" || instr == "push" || instr == "pop")
                {
                    if (instr == "clr") return 512; // "1000000000";
                    if (instr == "neg") return 513; // "1000000001" ;
                    if (instr == "inc") return 514; //"1000000010" ;
                    if (instr == "dec") return 515; // "1000000011";
                    if (instr == "asl") return 516; // "1000000100" ;
                    if (instr == "asr") return 517; // "1000000101";
                    if (instr == "lsr") return 518; // "1000000110" ;
                    if (instr == "rol") return 519; // "1000000111";
                    if (instr == "ror") return 520; // "1000001000" ;
                    if (instr == "rlc") return 521; // "1000001001" ;
                    if (instr == "rrc") return 522; // "1000001010" ;
                    if (instr == "jmp") return 523; // "1000001011";
                    if (instr == "call") return 524; // "1000001100" ;
                    if (instr == "push") return 525; // "1000001101" ;
                    if (instr == "pop") return 526; // "1000001110" ;

                }//clasa 4
                else if (instr == "clc" || instr == "clv" || instr == "clz" || instr == "cls" || instr == "ccc" ||
                    instr == "sec" || instr == "sev" || instr == "sez" || instr == "ses" ||
                    instr == "scc" || instr == "nop" || instr == "ret" || instr == "reti" ||
                    instr == "halt" || instr == "wait" || instr == "push pc" || instr == "pop pc" ||
                    instr == "push flag" || instr == "pop flag")
                    switch (instr)
                    {
                        case "clc":
                            return 49152; // "1100000000000000" ;
                            break;
                        case "clv":
                            return 49153; // "1100000000000001" ;
                            break;
                        case "clz":
                            return 49154; // "1100000000000010" ;
                            break;
                        case "cls":
                            return 49155; // "1100000000000011" ;
                            break;
                        case "ccc":
                            return 49156; // "1100000000000100" ;
                            break;
                        case "sec":
                            return 49157; // "1100000000000101" ;
                            break;
                        case "sev":
                            return 49158; // "11000000000000110" ;
                            break;
                        case "sez":
                            return 49159; // "1100000000000111";
                            break;
                        case "ses":
                            return 49160; // "1100000000001000" ;
                            break;
                        case "scc":
                            return 49161; // "1100000000001001" ;
                            break;
                        case "nop":
                            return 49162; // "1100000000001010" ;
                            break;
                        case "ret":
                            return 49163; // "1100000000001011" ;
                            break;
                        case "reti":
                            return 49164; // "1100000000001100" ;
                            break;
                        case "halt":
                            return 49165; // "1100000000001101" ;
                            break;
                        case "wait":
                            return 49166; // "1100000000001110" ;
                            break;
                        case "push pc":
                            return 49167;// "1100000000001111" ;
                            break;
                        case "pop pc":
                            return 49168; // "1100000000010000" ;
                            break;
                        case "push flag":
                            return 49169; // "1100000000010001" ;
                            break;
                        case "pop flag":
                            return 49170; // "1100000000010010" ;
                            break;
                    }//clasa 3
                else if (instr == "br" || instr == "bne" || instr == "beq" || instr == "bpl" ||
                    instr == "bmi" || instr == "bcs" || instr == "bcc" || instr == "bvs" ||
                    instr == "bvc")
                {
                    switch (instr)
                    {
                        case "br": return 160; break; //10100000
                        case "bne": return 161; break; //10100001
                        case "beq": return 162; break; //10100010
                        case "bpl": return 163; break; //10100011
                        case "bmi": return 164; break; //10100100
                        case "bcs": return 165; break; //10100101
                        case "bcc": return 166; break; //10100110
                        case "bvs": return 167; break; //10100111
                        case "bvc": return 168; break; //10101000

                    }
                }
                else if (instr == "end")
                {
                    MessageBox.Show("The End!");
                    return -2;
                }
                
            }
            
            return -1;
        }// end dec_opc

        //decodificare regidstru-------------------------------------------------------------
        int decod_reg(string R)
        {
           
            switch (R)
            {
                case "0": return 0; break;
                case "1": return 1;  break;
                case "2": return 2; break;
                case "3": return 3; break;
                case "4": return 4; break;
                case "5": return 5; break;
                case "6": return 6; break;
                case "7": return 7; break;
                case "8": return 8; break;
                case "9": return 9; break;
                case "10": return 10 ; break;
                case "11": return 11; break;
                case "12": return 12; break;
                case "13": return 13; break;
                case "14": return 14; break;
                case "15": return 15; break;
            }

            return -1;
        }
        
        //MOD ADRESARE
        int dec_mod_adresare(string R)
        {
            int index = 0;

            if (R.Contains('('))
            { 
                index = R.IndexOf('(');
                
                if (index == 0)
                    return 2; //indirecta
                else return 3;  //indexata
            }
            else
            {
                if (R.Contains('r'))
                    return 1; //directa
                else
                    return 0; //imediata
            }
            
        }

         int decod_microinstr(string micro_instr)
        {
            switch ( micro_instr)
            {
                //SBUS
                case "none": return 0; break;
                case "nop": return 0; break;
                case "PdIR[op]": return 1; break;
                case "PdIR[ind]": return 2; break;
                case "PdA": return 3; break;
                case "PdSP": return 4; break;
                case "Pd0s": return 5; break;
                case "Pd-1s": return 6; break;
                //DBUS
                case "PdM": return 1; break;
                case "PdnotM": return 2; break;
                case "PdRG": return 3; break;
                case "PdnotRG": return 4; break;
                case "PdPC": return 5; break;
                case "Pd0d": return 6; break;
                case "Pd-1d": return 7; break;
                //ALU
                case "SBUS": return 1; break;
                case "notSBUS": return 2; break;
                case "DBUS": return 3; break;
                case "SUM": return 4; break;
                case "AND": return 5; break;
                case "OR": return 6; break;
                case "XOR": return 7; break;
                case "INV": return 8; break;
                //sursaSBUS
                case "PdALU": return 1; break;
                case "PdIVR": return 2; break;
                case "PdFLAG": return 3; break;
                case "PDADR": return 4; break;
                //destRBUS
                case "PmIR": return 1; break;
                case "PmA": return 2; break;
                case "PmSP": return 3; break;
                case "PmM": return 4; break;
                case "PmRG": return 5; break;
                case "PmPC": return 6; break;
                case "PmADR": return 7; break;
                //mem op
                case "IFCH": return 1; break;
                case "READ": return 2; break;
                case "WRITE": return 3; break;
                //shift and other op
                case "DSA": return 1; break;
                case "DDA": return 2; break;
                case "RS": return 3; break;
                case "RD": return 4; break;
                case "RSC": return 5; break;
                case "RDC": return 6; break;
                case "DDL": return 7; break;
                case "+2PC": return 8; break;
                case "A(1)BVI": return 9; break;
                case "A(0)BVI": return 10; break;
                case "A(1)BE0": return 11; break;
                case "A(0)BE0": return 12; break;
                case "A(1)BE1": return 13; break;
                case "A(0)BE1": return 14; break;
                case "A(0)BP0": return 15; break;
                case "PdCOND": return 16; break;
                //sel cond ram
                case "C": return 1; break;
                case "Z": return 2; break;
                case "S": return 3; break;
                case "V": return 4; break;
                case "ACLOW": return 5; break;
                case "CIL": return 6; break;
                case "INT": return 7; break;
                //index
                case "01": return 1; break;
                case "10": return 2; break;
                case "11": return 3; break;
                //nonT/F
                case "0": return 0; break;
                //microadr de salt
                case "0000": return 0; break;

            }

            return -1;
        }

        private void ACLOWtxt_Click(object sender, EventArgs e)
        {
            ACLOW = 1;
        }

        private void MEMtxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          
            TextWriter txt = new StreamWriter("D:\\AC\\nou\\ProiectAC-new\\Binar.txt");
            txt.Write(OutputBinar.Text);
            txt.Close();
        }

        private void IRQcheck_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            TextWriter Mtxt = new StreamWriter("D:\\AC\\nou\\ProiectAC-new\\MicroBinar.txt");
            Mtxt.Write(MicroCodtxt.Text);
            Mtxt.Close();
        }

        private void Form1_Click(object sender, EventArgs e)
        {

        }

        private void BVIcheck_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void BVIcheck_Click(object sender, EventArgs e)
        {
            // BVI = 1;
            //BVItxt.Text = BVI.ToString();
           // BVItxt.BackColor = Color.LightSalmon;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Etichete_TextChanged(object sender, EventArgs e)
        {

        }

        //decodificare MA
        string index_decodificat = null;
        void afisare_MA_Rg(string R)
        {
            int indexi = 0, indexj = 0, Reg_decod = 0;
            string[] val_AX = null;
            string Rg = null;

            Rg = R;
            int Mod_Adresare = dec_mod_adresare(R);

            if (Mod_Adresare==2  || Mod_Adresare==3)
            {
                indexi = R.IndexOf('(');
                indexj = R.IndexOf(')');

                if (Mod_Adresare == 2) //indirecta
                {
                    Rg = Rg.Substring(R.IndexOf('(') + 2, R.IndexOf(')') - 2);
                }
                else if (Mod_Adresare == 3)//indexata
                {
                    Rg = Rg.Substring(R.IndexOf('(') + 2, R.IndexOf(')') - 3);
                    val_AX = R.Split('(');
                    string vaal = val_AX[0]; //convertiri pt a afisa index
                    int int_vaal = Int32.Parse(vaal);
                    index_decodificat = int_to_bin(int_vaal, 16);
                    
                }
                Reg_decod = decod_reg(Rg);

                INDEX_RG = Reg_decod;

                string pt_afisare = int_to_bin(Reg_decod, 4);
                OutputBinar.Text = OutputBinar.Text + pt_afisare + " ";

                indexi ++; // dupa prima paranteza
                indexj --; // inainte de a doua paranteza
                R = R.Substring(indexi, indexj);
                
            }
            if(Mod_Adresare==1)//direct
            {
                Rg = Rg.Substring(Rg.IndexOf('r') + 1);
                Reg_decod = decod_reg(Rg);

                INDEX_RG = Reg_decod;

                string pt_afisare = int_to_bin(Reg_decod, 4);
                OutputBinar.Text = OutputBinar.Text + pt_afisare + " ";
            }
            else if(Mod_Adresare == 0)//imediat
            {
                int numar = Int32.Parse(R);
                string numar_decod;
                numar_decod = int_to_bin(numar, 4);
                OutputBinar.Text = OutputBinar.Text + numar_decod + " ";
            }

            
        }
        
        //pentru salturi -> noul PC
        int get_new_PC (string Adresa )
        {
            int nou_PC=0, j=0;
            var nr_max_minii = Etichete.Lines.Count() -1 ; //pentru ca am un new line
           
             for (j = 0; j < nr_max_minii; j++) //j pt a parcurgefiecare linie din Etichete
             {
                string pentru_et = Etichete.Lines[j];
                string[] cuv_et = pentru_et.Split(' ');

                if (cuv_et[0].Length>0)
                {
                    cuv_et[0] = cuv_et[0].Substring(0, cuv_et[0].Length - 1);//sa scap de :

                    if (cuv_et[0] == Adresa)
                    {
                        string itermediar = cuv_et[1];
                        nou_PC = Convert.ToInt32(cuv_et[1]);
                    }
                index_linie++;
                }
                
                
            }
            //Verific.Text += "nou pc: " + nou_PC + "\n";
            return nou_PC;
        }
        
        private void MicroInstr_TextChanged(object sender, EventArgs e)
        {
            
        }

        //fetch operand-> pune in A si M operanzii
        int FO(int MA, string operand)
        {
           //cu aj dec_reg care are return int
           if( MA == 0)//imediata
            {
                int op = Int32.Parse(operand);
                return op;
            }
           else if (MA== 1)//directa
            {
                operand = operand.Replace("r", string.Empty);
                int op = Int32.Parse(operand);
                INDEX_RG = op;
                return R[op];
            }
            else if(MA ==2)
             {
                //mem*R[operand]
                operand = operand.Replace("(r", string.Empty);
                operand = operand.Replace(")", string.Empty);
                int op = Int32.Parse(operand);
                int pt_mem = R[op];
                op = Mm[pt_mem];

                MEMtxt.BackColor = Color.SandyBrown;
                MBUStxt.BackColor = Color.SandyBrown;
                ADR = op;
                ADRtxt.BackColor = Color.SandyBrown;
                return op;
             }
             else if (MA==3)
             {
                //index* mem(r[op])
                string Rg = operand;
                int indexi = operand.IndexOf('(');
                int indexj = operand.IndexOf(')');

                Rg = Rg.Substring(operand.IndexOf('(') + 2, operand.IndexOf(')') - 3);
                string[] operand_split = operand.Split('(');

                string string_index = operand_split[0]; 
                int index = Int32.Parse(string_index);
                

                int operand_decod = decod_reg(Rg);

                indexi = operand.IndexOf('(') + 1; // dupa prima paranteza
                indexj = operand.IndexOf(')') - 2; // inainte de a doua paranteza

                operand = operand.Substring(indexi, indexj);
                operand = operand.Replace("r", string.Empty);
                int op = Int32.Parse(operand);
                int suma = R[op] + index;
                // operand = null;

                MEMtxt.BackColor = Color.SandyBrown;
                ADR = suma;
                ADRtxt.Text = ADR.ToString();
                ADRtxt.BackColor = Color.SandyBrown;
                MBUStxt.BackColor = Color.SandyBrown;
                return suma;
             }
            return -1;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        string linie_micro;
        string Microcod(int instr, int clasa) //returneaza linie_micro
        {
            string[] toate_liniile_micro;


            if (clasa == 1)// 0- 6
            {
                toate_liniile_micro = System.IO.File.ReadAllLines(@"D:\\AC\\nou\\ProiectAC-new\\microcodClasa1.txt");
                linie_micro = toate_liniile_micro[instr]; //ca sa ajung cu indexul la 0
                int index = linie_micro.IndexOf(" ");
                linie_micro = linie_micro.Substring(index + 1);// sa scap de numar si spatiu

                return linie_micro;
            }
            else if (clasa == 2)//512- 526
            {
                toate_liniile_micro = System.IO.File.ReadAllLines(@"D:\\AC\\nou\\ProiectAC-new\\microcodClasa2.txt");
                instr -= 512;
                linie_micro = toate_liniile_micro[instr];
                int index = linie_micro.IndexOf(" ");
                linie_micro = linie_micro.Substring(index + 1);// sa scap de numar si spatiu
                return linie_micro;
            }
            else if (clasa == 3) //160-168
            {
                toate_liniile_micro = System.IO.File.ReadAllLines(@"D:\\AC\\nou\\ProiectAC-new\\microcodClasa3.txt");
                instr -= 160;
                linie_micro = toate_liniile_micro[instr];
                int index = linie_micro.IndexOf(" ");
                linie_micro = linie_micro.Substring(index + 1);// sa scap de numar si spatiu
                return linie_micro;
            }
            else if (clasa == 4) //49152  - 49170
            {
                toate_liniile_micro = System.IO.File.ReadAllLines(@"D:\\AC\\nou\\ProiectAC-new\\microcodClasa4.txt");
                instr -= 49152;
                linie_micro = toate_liniile_micro[instr];
                int index = linie_micro.IndexOf(" ");
                linie_micro = linie_micro.Substring(index + 1);// sa scap de numar si spatiu
                return linie_micro;;
            }
            return null;
        }
        
        void NOP() { }

        void F_SBUS(string mi) //mi = micro instructiune
        {   //pentru SBUS
            if (mi == "none") NOP();
            if (mi == "PdIR[op]") SBUS = -100;
            if (mi == "PdIR[ind]") SBUS = -200;
            if (mi == "PdA") SBUS = A;
            if (mi == "PdSP") SBUS = SP;
            if (mi == "Pd0s") SBUS = 0;
            if (mi == "Pd-1s") SBUS = -1;

            Atxt.Text = A.ToString();
            Atxt.BackColor = Color.SandyBrown;
            SBUSrosu.BackColor = Color.SandyBrown;
        }

        void F_DBUS(string mi)
        {   //pentru DBUS
            if (mi == "none") NOP();
            if (mi == "PdM") DBUS = M;
            if (mi == "PdnotM") DBUS = -M;
            if (mi == "PdRG") DBUS = R[INDEX_RG];
            if (mi == "PdnonRG") DBUS = -R[INDEX_RG];
            if (mi == "PdPC") DBUS = PC;
            if (mi == "Pd0d") DBUS = 0;
            if (mi == "Pd-1d") DBUS = -1;

            
            Mtxt.Text = M.ToString();
            Mtxt.BackColor = Color.SandyBrown;
            DBUSrosu.BackColor = Color.SandyBrown;
        }

       void F_ALU(string mi)
        {  //pentru ALU
            if (mi == "SBUS")
            {
                ALU = SBUS;
                ALUtxt.Text = "SBUS"+"\n"+ALU;
                ALUtxt.BackColor = Color.SandyBrown;
            }
            else if (mi == "notSBUS")
            {
                ALU = -SBUS;
                ALUtxt.Text = "notSBUS" + "\n" + ALU;
                ALUtxt.BackColor = Color.SandyBrown;
            }
            else if (mi == "DBUS")
            {
                ALUtxt.Text = "DBUS" + "\n" + ALU;
                ALUtxt.BackColor = Color.SandyBrown;
                ALU = DBUS;
            }
            else if (mi == "SUM")
            {
                ALUtxt.Text = "SUM" + "\n" + SBUS + " + " + DBUS;
                ALUtxt.BackColor = Color.SandyBrown;
                ALU = SBUS + DBUS;
            }
            else if (mi == "AND")
            {
                ALUtxt.Text = "AND";
                ALUtxt.BackColor = Color.SandyBrown;
                ALU = SBUS & DBUS;
            }
            else if (mi == "OR")
            {
                ALUtxt.Text = "OR";
                ALUtxt.BackColor = Color.SandyBrown;
                ALU = SBUS | DBUS;
            }
            else if( mi =="XOR")
            {
                ALUtxt.Text = "OR";
                ALUtxt.BackColor = Color.SandyBrown;
                ALU = SBUS | DBUS;
            }
            else if (mi == "none") NOP();

            //pentru setarea bitilor Cout, Sr, Zr,Vr
            if (ALU == 0)
            {
                Zr = 1;
            }
            if (ALU < 0) Sr = 1;
            Vr = 0; Cout = 0;
        }
          
        void F_S_RBUS(string mi)
        {
            if (mi == "none") NOP();
            if (mi == "PdALU") RBUS = ALU;
            if (mi == "PdIVR") RBUS = IVR;
            if (mi == "PdFLAG") RBUS =FLAGS;
            if (mi == "PdAdr") RBUS = ADR;
            if (mi == "PdZ") RBUS = Z;

            RBUSrosu.BackColor = Color.SandyBrown;
        }

        void F_D_RBUS(string mi)
        {
            if (mi == "none") NOP();
            if (mi == "PmA")
            {
                A = RBUS;
                Atxt.Text = A.ToString();
                Atxt.BackColor = Color.LightSeaGreen;
                if (index3 != 0) R[INDEX_RG] = A;
                
            }
            if (mi == "PmSP") SP= RBUS;
            if (mi == "PmM") M= RBUS;
            if (mi == "PmRG") R[INDEX_RG] = RBUS;
            if (mi == "PmPC") PC = RBUS;
            if (mi == "PmAdr") ADR = RBUS;
            
        }

        void Other(string mi)
        {
            if (mi == "nop") NOP();
            if (mi == "Cin") ALU += C;
            if (mi == "PdCOND")
            {
                //stetez biti conditie corespunzator
                C = Cout; Z = Zr; S = Sr; V = Vr;
                Ctxt.BackColor = Color.SandyBrown; Ztxt.BackColor = Color.SandyBrown; 
                Stxt.BackColor = Color.SandyBrown; Vtxt.BackColor = Color.SandyBrown;
                Ctxt.Text = C.ToString(); Ztxt.Text = Z.ToString();
                Stxt.Text = S.ToString(); Vtxt.Text = V.ToString();
            }
            if (mi == "INTA") NOP();
            if (mi == "-2SP") SP -= 2;
            if (mi == "+SP") SP += 2;
            if (mi == "PdFLAG") RBUS=FLAGS;
            if (mi == "PmFLAG") FLAGS = RBUS;
            
        }

        void Mem_op(string mi)
        {
            if (mi == "nop") NOP();
            if (mi == "IFCH")
            {
                IFCH = 1;
                memReadtxt.Text = "1"; memReadtxt.BackColor = Color.LightSalmon;
            }
            if (mi == "READ")
            {
                READ = 1;
                memReadtxt.Text = "1"; memReadtxt.BackColor = Color.LightSalmon;
            }
            if (mi == "WRITE")
            {
                WRITE = 1;
                memWritetxt.Text = "1"; memWritetxt.BackColor = Color.LightSalmon;
            }
        }

        void Shift_and_Other(string mi)
        {
            if (mi == "nop") NOP();
            if (mi == "DSA") NOP();
            if (mi == "DDA") NOP();
            if (mi == "RS") NOP();
            if (mi == "RD") NOP();
            if (mi == "RSC") NOP();
            if (mi == "RDC") NOP();
            if (mi == "DDL") NOP();
            if (mi == "A(1)BVI") BVI=0;
            if (mi == "A(1)BE0") BE0=1;
            if (mi == "A(1)BE1") BE1 = 1;
            if (mi == "A(0)BI,") Bi=0;
            if (mi == "A(0)BE") BE=0;
        }

        void Succesor(string mi)
        {
            if (mi == "none") NOP();
        }

        void Selectie_index(string mi)
        {
            // if(mi=="0001") INDEX1;
            // if(mi=="0010") INDEX2;
            // if(mi=="0011") INDEX3;
        }
        
        void microcod_pas_cu_pas(string linie)//liniemicro pas cu pas
        {
            string Mdecode;
            int dec;
            Mcuv = linie.Split(' ');
            

            if (micro_index < Mcuv.Length-1)
            {
                MicroInstr.Text += Mcuv[micro_index] + "   ";

                switch (micro_index)
                {
                    case 0: F_SBUS(Mcuv[micro_index]); break;
                    case 1: F_DBUS(Mcuv[micro_index]); break;
                    case 2: F_ALU(Mcuv[micro_index]); break;
                    case 3: F_S_RBUS(Mcuv[micro_index]); break;
                    case 4: F_D_RBUS(Mcuv[micro_index]); break;
                    case 5: Other(Mcuv[micro_index]); break;
                    case 6: Mem_op(Mcuv[micro_index]); break;
                    case 7: Shift_and_Other(Mcuv[micro_index]); break;
                    case 8: Succesor(Mcuv[micro_index]); break;
                    case 9: Selectie_index(Mcuv[micro_index]); break;
                }
                
                //decod microinstr
                //pe 4b
                 if(micro_index == 0 || micro_index == 1 || micro_index == 2 || micro_index == 3 || micro_index == 4 || micro_index == 11)
                {
                    dec = decod_microinstr(Mcuv[micro_index]);
                    Mdecode = int_to_bin(dec, 4);
                    MicroCodtxt.Text += Mdecode + " ";

                    MIR += Mdecode;
                    //MicroCodtxt.Text += "MIR: " + MIR + " ";
                }
                 else if(micro_index == 5 || micro_index == 7 || micro_index == 8)
                {
                    dec = decod_microinstr(Mcuv[micro_index]);
                    Mdecode = int_to_bin(dec, 5);
                    MicroCodtxt.Text += Mdecode + " ";
                    MIR += Mdecode;
                   // MicroCodtxt.Text += "MIR: " + MIR + " ";
                }
                else if(micro_index == 6 || micro_index== 9)
                {
                    dec = decod_microinstr(Mcuv[micro_index]);
                    Mdecode = int_to_bin(dec, 2);
                    MicroCodtxt.Text += Mdecode + " ";
                    MIR += Mdecode;
                   // MicroCodtxt.Text += "MIR: " + MIR + " ";
                }
                else if( micro_index == 10)
                {
                    dec = decod_microinstr(Mcuv[micro_index]);
                    Mdecode = int_to_bin(dec, 1);
                    MicroCodtxt.Text += Mdecode + " ";
                    MIR += Mdecode;
                }

                micro_index++;

                
            }
            else MicroInstr.Text += "\n-------------\n" ;
            //MIR = MicroCodtxt.Lines[micro_index];
        }
        
        private void btnMStep_Click(object sender, EventArgs e)
        {
            microcod_pas_cu_pas(LINIE);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        void PowerFail()
        {
            MessageBox.Show("ACLOW, jumping to PowerFail");
            BE0 = 1;
            //IRQ check
            //Handler();
            this.Close();
        }

        void Punct_Interuptibil()
        {
            if (BVIcheck.Checked == true && IRQcheck.Checked == true)
            {
                BVI = 1;
                MessageBox.Show("Interruped ocuured. Working on it..");
                //fa ceva IRET
            }
            else BVI = 0;

            BVItxt.Text = BVI.ToString();
        }

        void Illegal()
        {
            MessageBox.Show("Illegal instruction");
            BE1 = 1;
            //IRQ check
            //Handler();
        }

        public void Asamblor()
        {
            int lungime = toate_liniile.Length, clasa;
            string linie;

            index_linie = PC / 4;
            if (index_linie < lungime)
            {
                InputAsm.Text = InputAsm.Text + toate_liniile[index_linie] + "\n";
                linie = toate_liniile[index_linie];
                string[] cuvSeparat = linie.Split(' '); // baga in vector cuvintele separate
                int ct = 0, ma_rs, ma_rd;
                char ultimul_caracter = cuvSeparat[ct][cuvSeparat[ct].Length - 1]; //vf daca am eticheta ( daca se termina cu : )

                ADR = PC;
                PC += 4;
                PCtxt.Text = PC.ToString();
                PCtxt.BackColor = Color.GreenYellow;

                if (ultimul_caracter == ':')// am eticheta:_
                {
                    index_et++;
                    ct++;
                }

                //---------------------------------------------------------------------------------------------------------
                instructiune = cuvSeparat[ct];
                nrInstr = Dec_Opcode(instructiune);
                
                string opcode = int_to_bin(nrInstr, 4);//!!!! atentie dep de clasa
                //string binar = int_to_bin(nrInstr, 4);
                OutputBinar.Text += opcode + " ";
                IR = opcode;
                if (IR.Length < 16)
                    IR = IR.PadRight(16, '1');
                IR = Reverse(IR);
                clasa = getClasa(IR);

                
                //---------------------------------------------------------------------------------------------------------

                if (clasa == 1)
                {
                    Rd = cuvSeparat[ct + 1]; //reg drest
                    Rs = cuvSeparat[ct + 2]; //reg sursa

                    //pt reg sursa
                    ma_rs = dec_mod_adresare(Rs);
                    opcode = int_to_bin(ma_rs, 2); OutputBinar.Text += opcode + " ";
                    afisare_MA_Rg(Rs);
                    //pt reg destinatie
                    ma_rd = dec_mod_adresare(Rd);
                    opcode = int_to_bin(ma_rd, 2); OutputBinar.Text += opcode + " ";
                    afisare_MA_Rg(Rd);
                    if (ma_rs == 3) OutputBinar.Text += "\n" + "i" + index_decodificat;

                }
                else if (clasa == 2)
                {
                    Rd = cuvSeparat[ct + 1]; //reg drest
                    ma_rd = dec_mod_adresare(Rd);
                    opcode = int_to_bin(ma_rd, 2); OutputBinar.Text += opcode + " ";
                    afisare_MA_Rg(Rd); // mod de adresare si registru
                    //if (ma_rs == 3) OutputBinar.Text += "\n" + "i" + index_decodificat;

                }
                else if (clasa == 3)
                {
                    ADRtxt.Text = ADR.ToString();
                    ADRtxt.BackColor = Color.SandyBrown;
                    Adresa = cuvSeparat[ct + 1];

                    if(nrInstr == 161 && Z==0) //bne, sar daca Z=0
                    {
                        ADR = get_new_PC(Adresa);
                        string offset = int_to_bin(ADR, 8);
                        OutputBinar.Text += offset;
                        ADR -= 4;//pt ca am pc+4 mai jos
                    }
                }
                // else if(clasa==4)
                //{

                //}
                
                string verific_index = OutputBinar.Lines[index_IR];
                if (verific_index.Contains("i") == false)
                    IR = OutputBinar.Lines[index_IR];
                else
                {
                    index_IR++;
                    IR = OutputBinar.Lines[index_IR];
                }


                IR = IR.Replace(" ", string.Empty);
                IRtxt.Text = IR.ToString();
                IRtxt.BackColor = Color.GreenYellow;
                IR = Reverse(IR);

                ADRtxt.Text = ADR.ToString();
                ADRtxt.BackColor = Color.GreenYellow;
               // PC += 4;

                index_IR++;
                OutputBinar.Text += "\n";
            }
        }

        //int ma;
        int index1, index2, index3;
        public void Secventiator()
        {
            //if ACLOW jump powerfail (->LdMAR) else step (-> +1MAR)
            if (ACLOW == 1) PowerFail(); //else step
            if (ILLEGAL == 1) Illegal(); //else step
            
        

            if (index1 == 1)
            {
                index2 = dec_mod_adresare(Rs);
                A = FO(index2, Rs);

                if (index2 == 3)
                    A = Mm[ADR];

                index3 = dec_mod_adresare(Rd);
                M = FO(index3, Rd);
                
                LINIE = Microcod(nrInstr, index1);
                microcod_pas_cu_pas(LINIE);

            }
            else if(index1 == 2)
            {
                index3 = dec_mod_adresare(Rd);
                M = FO(index3, Rd);

                Mtxt.Text = M.ToString();
                Mtxt.BackColor = Color.SandyBrown;

                LINIE = Microcod(nrInstr, index1);
                microcod_pas_cu_pas(LINIE);
            }
            else if(index1 == 3)
            {
                LINIE = Microcod(nrInstr, index1);
                microcod_pas_cu_pas(LINIE);
            }
            else if(index1 == 4)
            {
                LINIE = Microcod(nrInstr, index1);
                microcod_pas_cu_pas(LINIE);
            }

            R0txt.Text = R[0].ToString();
            R1txt.Text = R[1].ToString();
            R2txt.Text = R[2].ToString();
            R3txt.Text = R[3].ToString();
            R4txt.Text = R[4].ToString();
            R5txt.Text = R[5].ToString();
            R6txt.Text = R[6].ToString();
            R7txt.Text = R[7].ToString();
            R8txt.Text = R[8].ToString();
            R9txt.Text = R[9].ToString();
            R10txt.Text = R[10].ToString();
            R11txt.Text = R[11].ToString();
            R12txt.Text = R[12].ToString();
            R13txt.Text = R[13].ToString();
            R14txt.Text = R[14].ToString();
            R15txt.Text = R[15].ToString();
            //BVItxt.Text = BVI.ToString();
            Ctxt.Text = C.ToString();
            Ztxt.Text = Z.ToString();
            Stxt.Text = S.ToString();
            Vtxt.Text = V.ToString();

            
        }
        private void btnStep_Click(object sender, EventArgs e)
        {
            
            micro_index = 0; //pt resetare index, sa nu contiune microinsstr precedenta, daca nu s a terminat
            MicroInstr.Text += "\n";
            MicroCodtxt.Text += "\n";
           // MicroCodtxt.Text += "MIR: " + MIR + " ";
            MEMtxt.Text = Mm[0].ToString() +"\n"+ Mm[1].ToString() + "\n" + Mm[2].ToString() + "\n" + Mm[3].ToString() + "\n" + Mm[4].ToString();
            ALUtxt.BackColor = Color.LightGray;
            ALUtxt.Text = " ";

            SBUSrosu.BackColor = Color.White;
            DBUSrosu.BackColor = Color.White;
            RBUSrosu.BackColor = Color.White;
            MBUStxt.BackColor = Color.White;

            Ztxt.BackColor = Color.White; Ctxt.BackColor = Color.White; Stxt.BackColor = Color.White; Vtxt.BackColor = Color.White;
            BVItxt.BackColor = Color.White;
            Atxt.BackColor = Color.White;
            Mtxt.BackColor = Color.White;
            ADRtxt.BackColor = Color.White;
            PCtxt.BackColor = Color.White;
            MEMtxt.BackColor = Color.White;
            ADRtxt.BackColor = Color.White;

            Asamblor();
            Secventiator();
            Punct_Interuptibil();
        }
            private void Form1_Load(object sender, EventArgs e)
        {
            iau_etichete();
        }
    }
}
