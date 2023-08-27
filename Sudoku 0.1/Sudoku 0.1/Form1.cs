using System.Runtime.CompilerServices;

namespace Sudoku_0._1
{
    public partial class Sudoku : Form
    {
        public Sudoku()
        {
            InitializeComponent();
            Difficulty_Settings(2);
            Generate_Panels();
            Generate_Labels();
            Generate_Grid();
            Init_Input_Buttons();
            current_theme.Set_Theme("Intunecat");
            Update_Theme();
            timer1.Start();
            this.KeyPress += new KeyPressEventHandler(Key_Pressed);
        }
        
        //variables
        Label[,] boxes = new Label[10, 10];
        Label active_box = new Label();
        Font font_label_default = new Font("Consolas", 16, FontStyle.Bold);
        Font font_label_completed = new Font("Consolas", 16);
        Font font_menu_default = new Font("Consolas", 9);
        TableLayoutPanel[,] table = new TableLayoutPanel[4, 4];
        Themes current_theme = new Themes();
        int sec, min;
        int selected_row=0, selected_col=0;
        int[,] grid = new int[10, 10];
        int[,] grid_default = new int[10, 10];
        int[,] grid_solving = new int[10, 10];
        int[] rnd_row = new int[10];
        int desired_empty_cells = 40;
        int remaining_cells;
        int available_hints;
        private ToolStripItemClickedEventHandler menu_ItemClicked;
        

        public void Generate_Labels()
        {
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    
                    boxes[i, j] = new Label();
                    int rez = i * 10 + j * 1;
                    boxes[i, j].Name = "casuta" + rez.ToString();
                    boxes[i, j].Width = 59;
                    boxes[i, j].Height = 59;
                    boxes[i, j].AutoSize = false;
                    boxes[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    int xi = i % 3, yi = j % 3;
                    if (xi == 0) xi = 3;
                    if (yi == 0) yi = 3;
                    table[(i-1)/3+1, (j-1)/3+1].Controls.Add(boxes[i, j], xi-1, yi-1);
                    boxes[i, j].Click += new EventHandler(Boxes_Click);
                }
            }
        }

        public void Generate_Panels()
        {
            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    table[i, j] = new TableLayoutPanel();
                    table[i, j].Name = "panel" + i.ToString() + j.ToString();
                    table[i, j].Width = 180;
                    table[i, j].Height = 180;
                    table[i, j].AutoSize = false;
                    table[i, j].BorderStyle = BorderStyle.FixedSingle;
                    table[i, j].BackColor = current_theme.cell_background_default; //cell_background_default

                    //columns
                    table[i, j].ColumnCount = 3;
                    table[i, j].ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                    table[i, j].ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                    table[i, j].ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                    
                    table[i, j].CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

                    //rows
                    table[i, j].RowCount = 3;
                    table[i, j].RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
                    table[i, j].RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
                    table[i, j].RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));

                    Panel_MainGrid.Controls.Add(table[i, j], i-1, j-1);
                }
            }
            lbl_hints.Text = Convert.ToString(available_hints);
        }

        public void Key_Pressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '1' && e.KeyChar <= '9' && selected_row != 0 && selected_col != 0)
            {
                Active_Box_Value(e.KeyChar - '0');
            }
            else if (e.KeyChar == (char)Keys.Back) //backspace
            {
                Active_Box_Value(0);
            }
            e.Handled = true;
        }

        public void Boxes_Click(object sender, EventArgs e)
        {
            Label box = sender as Label;
            if (box.Font == font_label_completed)
            {
                Update_Active_Box(box);
            }
        }
        public void Update_Active_Box(Label new_active)
        {
            selected_row = 1;
            selected_col = 1;
            active_box.BackColor = current_theme.cell_background_active; //cell_background_active
            active_box = new_active;
            active_box.BackColor = current_theme.cell_background_selected; //cell_background_selected
        }
        
        public void Active_Box_Value(int x)
        {
            selected_row = active_box.Name[6] - '0';
            selected_col = active_box.Name[7] - '0';
            if (grid_solving[selected_row, selected_col] != 0 && (x != 0))
                remaining_cells++;
            grid_solving[selected_row, selected_col] = x;
            if (x!=0)
            {
                active_box.Text = x.ToString();
                if (remaining_cells!=0)
                    remaining_cells--;
            }
            else
            {
                active_box.Text = "";
                remaining_cells++;
            }
            if (remaining_cells==0)
            {
                if (Check_Solution(grid_solving)==true)
                {
                    MessageBox.Show("Felicitari! Ai rezolvat sudoku-ul!");
                }
                else
                {
                    MessageBox.Show("Ai gresit! Incearca din nou!");
                }
            }
        }
        
        public void Init_Input_Buttons()
        {
            btn_1.Click += new EventHandler(Input_Buttons_Click);
            btn_2.Click += new EventHandler(Input_Buttons_Click);
            btn_3.Click += new EventHandler(Input_Buttons_Click);
            btn_4.Click += new EventHandler(Input_Buttons_Click);
            btn_5.Click += new EventHandler(Input_Buttons_Click);
            btn_6.Click += new EventHandler(Input_Buttons_Click);
            btn_7.Click += new EventHandler(Input_Buttons_Click);
            btn_8.Click += new EventHandler(Input_Buttons_Click);
            btn_9.Click += new EventHandler(Input_Buttons_Click);
            btn_clear0.Click += new EventHandler(Input_Buttons_Click);
        }

        public void Input_Buttons_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int x = btn.Name[btn.Name.Length - 1] - '0';
            Active_Box_Value(x);
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            DialogResult t;
            t = MessageBox.Show("Sigur doriti sa parasiti apllicatia?", "Iesire", MessageBoxButtons.YesNo);
            if (t == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btn_About_Click(object sender, EventArgs e)
        {
            string despre = "Sudoku\n\nAutor: Terebent Roxana\n\nData: 2022-2023";
            string how_to_play = "\n\nSudoku este un joc de logica, care presupune completarea unei grile de 9x9 cu cifrele de la 1 la 9, astfel incat in fiecare linie, coloana si regiune de 3x3 sa nu se repete aceeasi cifra. Fiecare grila reprezinta un puzzle cu solutie unica.\n\n";
            how_to_play += "Fiecare nivel de dificultate este conceput relativ la numarul de indicii completate, care variaza de la 26 la 56 casute.\n\n";
            how_to_play += "Toate puzzle-urile sunt generate aleatoriu, fapt ce poate influenta dificultatea reala de rezolvare pentru un om.";

            MessageBox.Show(despre+how_to_play, "Despre joc");
        }

        private void btn_NewGame_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Novice");
            menu.Items.Add("Usor");
            menu.Items.Add("Mediu");
            menu.Items.Add("Dificil");
            menu.Items.Add("Expert");

            menu.Show(btn_NewGame, new Point(0, btn_NewGame.Height));
            menu.ItemClicked += new ToolStripItemClickedEventHandler(menu_ItemClicked);
            menu.Font = font_menu_default;
            menu.BackColor = current_theme.background_default;
            menu.ForeColor = current_theme.forecolor_default;
            int difficulty = 2;

            void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
            {
                menu.Close();
                bool ok = true;
                switch (e.ClickedItem.Text)
                {
                    case "Novice":
                        difficulty = 1;
                        
                        break;
                    case "Usor":
                        difficulty = 2;
                        
                        break;
                    case "Mediu":
                        difficulty = 3;
                        
                        break;
                    case "Dificil":
                        difficulty = 4;
                        
                        break;
                    case "Expert":
                        difficulty = 5;
                        
                        DialogResult t;
                        string msg = "ATENTIE! Generarea unui puzzle \"Expert\" poate dura mai mult timp. Sigur doriti sa continuati?";
                        t = MessageBox.Show(msg, "Atentie", MessageBoxButtons.YesNo);
                        if (t == DialogResult.No)
                            ok = false;
                        break;
                    default:
                        difficulty = 2;
                        break;
                }    
                if (ok)
                {
                    Difficulty_Settings(difficulty);
                    lbl_hints.Text = available_hints.ToString();
                    Generate_Grid();
                    sec = 0; min = 0;
                }
            }
        }

        private void btn_Hint_Click(object sender, EventArgs e)
        {
            if (remaining_cells != 0 && available_hints!=0)
            {
                Random random = new Random();
                int x = random.Next(1, 10);
                int y = random.Next(1, 10);
                while (grid_solving[x, y] != 0)
                {
                    x = random.Next(1, 10);
                    y = random.Next(1, 10);
                }
                grid_solving[x, y] = grid[x, y];
                boxes[x, y].Text = grid[x, y].ToString();
                remaining_cells--;
                available_hints--;
                lbl_hints.Text = Convert.ToString(available_hints);

            }
            else
            {
                MessageBox.Show("Nu mai sunt indicii disponibile!", "Atentie");
            }
        }

        private void btn_Themes_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Albastru");
            menu.Items.Add("Roz");
            menu.Items.Add("Violet");
            menu.Items.Add("Galben");
            menu.Items.Add("Portocaliu");
            menu.Items.Add("Verde");
            menu.Items.Add("Luminos");
            menu.Items.Add("Intunecat");

            menu.Show(btn_Themes, new Point(0, btn_Themes.Height));
            menu.ItemClicked += new ToolStripItemClickedEventHandler(menu_ItemClicked);
            menu.Font = font_menu_default;
            menu.BackColor = current_theme.background_default;
            menu.ForeColor = current_theme.forecolor_default;

            void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
            {
                menu.Close();
                current_theme.Set_Theme(e.ClickedItem.Text);
                Update_Theme();
            }
        }   

        private void timer1_Tick(object sender, EventArgs e)
        {
            sec++;
            if (sec == 60)
            {
                min++;
                sec = 0;
            }
            if (min<10)
                lbl_timer.Text = "0" + min.ToString() + ":";
            else
                lbl_timer.Text = min.ToString() + ":";
            if (sec < 10)
                lbl_timer.Text += "0" + sec.ToString();
            else
                lbl_timer.Text += sec.ToString();
        }

        private void btn_ShowSolution_Click(object sender, EventArgs e)
        {
            DialogResult t;
            t = MessageBox.Show("Sigur doriti sa afisati solutia?", "Solutie", MessageBoxButtons.YesNo);
            if (t == DialogResult.Yes)
            {
                for (int i = 1; i < 10; i++)
                {
                    for (int j = 1; j < 10; j++)
                    {
                        if (boxes[i, j].Text != grid[i, j].ToString() && boxes[i, j].Text != "")
                            boxes[i, j].ForeColor = current_theme.forecolor_wrong; //forecolor_wrong
                        boxes[i, j].Text = grid[i, j].ToString();
                        grid_solving[i, j] = grid[i, j];
                        remaining_cells = 0;
                    }
                }
            }
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            Reset_labels();
        }
        
        //populates labels with numbers from grid_default
        private void Reset_labels()
        {
            remaining_cells = 0;
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    if (grid_solving[i, j] != grid_default[i, j])
                        grid_solving[i, j] = grid_default[i, j];
                    boxes[i, j].ForeColor = current_theme.forecolor_default; //forecolor_default
                    if (grid_default[i, j]!=0)
                    {
                        boxes[i, j].Text = grid_default[i, j].ToString();
                        boxes[i, j].BackColor = current_theme.cell_background_default; //cell_background_default
                        boxes[i, j].Font = font_label_default;
                    }
                    else
                    {
                        remaining_cells++;
                        boxes[i, j].Text = "";
                        boxes[i, j].BackColor = current_theme.cell_background_active; //cell_background_active
                        boxes[i, j].Font = font_label_completed;
                    }
                }
            }
        }
        public void Update_Theme()
        {
            this.BackColor = current_theme.background_default;
            active_box.BackColor = current_theme.cell_background_selected;
            Reset_labels(); //clears grid and reattributes colors
            //repopulates grids with prev completed values
            for (int i = 1; i <= 9; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    if (grid_solving[i, j] != 0)
                        boxes[i, j].Text = grid_solving[i, j].ToString();
                    else
                        boxes[i, j].Text = "";
                }
            }
            //updates button backgrounds
            Color crt_color = current_theme.button_background_default;
            btn_1.BackColor = crt_color;
            btn_2.BackColor = crt_color;
            btn_3.BackColor = crt_color;
            btn_4.BackColor = crt_color;
            btn_5.BackColor = crt_color;
            btn_6.BackColor = crt_color;
            btn_7.BackColor = crt_color;
            btn_8.BackColor = crt_color;
            btn_9.BackColor = crt_color;
            btn_clear0.BackColor = crt_color;

            btn_Exit.BackColor = crt_color;
            btn_NewGame.BackColor = crt_color;
            btn_Reset.BackColor = crt_color;
            btn_Hint.BackColor = crt_color;
            btn_Themes.BackColor = crt_color;
            btn_About.BackColor = crt_color;
            btn_ShowSolution.BackColor = crt_color;

            //updates labels and text color
            crt_color = current_theme.forecolor_default;
            lbl_Sudoku.ForeColor = crt_color;
            gpB_Menu.ForeColor = crt_color;
            lbl_timer.ForeColor = crt_color;
            lbl_timp.ForeColor = crt_color;
            btn_1.ForeColor = crt_color;
            btn_2.ForeColor = crt_color;
            btn_3.ForeColor = crt_color;
            btn_4.ForeColor = crt_color;
            btn_5.ForeColor = crt_color;
            btn_6.ForeColor = crt_color;
            btn_7.ForeColor = crt_color;
            btn_8.ForeColor = crt_color;
            btn_9.ForeColor = crt_color;
            btn_clear0.ForeColor = current_theme.forecolor_wrong;

        }

        //symmetries to be applied
        public void Horizontal_Symmetry(ref int i, ref int j)
        {
            int aux = i;
            i = 10 - aux;
        }
        public void Vertical_Symmetry(ref int i, ref int j)
        {
            int aux = j;
            j = 10 - aux;
        }
        public void Diagonal_Symmetry_1(ref int i, ref int j)
        {
            int aux = i;
            i = j;
            j = aux;
        }
        public void Diagonal_Symmetry_2(ref int i, ref int j)
        {
            int aux = i;
            i = 10 - j;
            j = 10 - aux;
        }

        public void Rotational_Symmetry(ref int i, ref int j)
        {
            int aux = i;
            i = 10 - j;
            j = aux;
        }

        //difficulty settings
        public void Difficulty_Settings(int difficulty)
        {
            switch (difficulty)
            {
                case 1: //very easy
                    desired_empty_cells = 35;
                    available_hints = 3;
                    break;
                case 2: //easy
                    desired_empty_cells = 40;
                    available_hints = 3;
                    break;
                case 3: //medium
                    desired_empty_cells = 45;
                    available_hints = 3;
                    break;
                case 4: //hard
                    desired_empty_cells = 50;
                    available_hints = 5;
                    break;
                case 5: //very hard
                    desired_empty_cells = 55;
                    available_hints = 5;
                    break;
                default:
                    desired_empty_cells = 40;
                    available_hints = 5;
                    break;
            }
            
        }

        public bool Check_Solution(int[,] grid_check)
        {
            for (int i = 1; i <= 9; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    if (grid_check[i, j] != grid[i, j])
                        return false;
                }
            }
            return true;
        }

        //sudoku validator
        public bool IsValid(int[,] grid, int row, int col, int num)
        {
            //check row
            for (int i = 1; i < 10; i++)
            {
                if (grid[row, i] == num)
                {
                    return false;
                }
            }

            //check column
            for (int i = 1; i < 10; i++)
            {
                if (grid[i, col] == num)
                {
                    return false;
                }
            }

            //check 3x3
            int startRow = (row-1)/3*3;
            int startCol = (col-1)/3*3;

            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    if (grid[i + startRow, j + startCol] == num)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //generate random line
        public void Generate_Line()
        {
            int[] numbers = new int[10];
            for (int i = 1; i < 10; i++)
            {
                numbers[i] = i;
            }

            Random rnd = new Random();
            for (int i = 1; i < 10; i++)
            {
                int index = rnd.Next(1, 10);
                int aux = numbers[i];
                numbers[i] = numbers[index];
                numbers[index] = aux;
            }

            for (int i = 1; i < 10; i++)
            {
                rnd_row[i]  = numbers[i];
            }
        }

        //sudoku grid generator
        private void Generate_Grid()
        {
            //generates grids
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    grid[i, j] = new int();
                    grid[i, j] = 0;
                    grid_default[i, j] = new int();
                    grid_default[i, j] = 0;
                    grid_solving[i, j] = new int();
                    grid_solving[i, j] = 0;
                }
            }
            
            //populates solved grid
            Solve(grid);
            //grid is completed sudoku, grid_default is the puzzle
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    grid_default[i, j] = grid[i, j];
                }
            }

            //random symmetry to apply
            Random rnd = new Random();
            int sym = rnd.Next(0, 6);

            //number of tries to empty cells while keeping unique solution
            int no_iterations = 0;

            int[,] gr = new int[4, 4]; //marks 3x3 squares with missing box
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    gr[i, j] = new int();
                    gr[i, j] = 0;
                }
            }
            int used_gr=0;

            for (int cnt = 0; cnt<desired_empty_cells; cnt++)
            {
                int i, j, i1, j1, gri, grj, no_it=0;
                //ensures that there is no full 3x3 square 
                do
                {
                    i = rnd.Next(1, 10); i1 = i;
                    j = rnd.Next(1, 10); j1 = j;
                    gri = (i - 1) / 3;
                    grj = (j - 1) / 3;
                    no_it++;
                } while (gr[gri, grj]!=0 && used_gr<9 && no_it<20);
                
                if (gr[gri, grj]==0)
                {
                    gr[gri, grj] = 1;
                    used_gr++;
                }
                
                if (grid_default[i, j] != 0)
                {
                    grid_default[i, j] = 0; //deletes random cell
                    switch (sym)
                    {
                        case 0: //no symmetry
                            cnt--;
                            break;
                        case 1:
                            Horizontal_Symmetry(ref i, ref j);
                            break;
                        case 2:
                            Vertical_Symmetry(ref i, ref j);
                            break;
                        case 3:
                            Diagonal_Symmetry_1(ref i, ref j);
                            break;
                        case 4:
                            Diagonal_Symmetry_2(ref i, ref j);
                            break;
                        case 5:
                            int no_rotations = rnd.Next(1, 4);
                            for (int k = 1; k <= no_rotations; k++)
                                Rotational_Symmetry(ref i, ref j); //applies rot sym a random no of times
                            break;
                    }
                    grid_default[i, j] = 0; //deletes corresponding cell after applying symmetry
                    cnt++;
                }
                else cnt--;
                if (Count_Solutions(grid_default) > 1)
                {
                    no_iterations++;
                    //resets deleted cells
                    grid_default[i1, j1] = grid[i1, j1];
                    cnt--;
                    if (sym != 0)
                    {
                        grid_default[i, j] = grid[i, j];
                        cnt--;
                    }
                }
                else no_iterations = 0;
                if (no_iterations > 100)
                    break;
            }

            //grid_solving is the grid containing puzzle with user input
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    grid_solving[i, j] = grid_default[i, j];
                    if (grid_solving[i, j] == 0)
                        remaining_cells++;
                }
            }
            Reset_labels();
        }

        

        //sudoku solver, populates grid with a randomly solved sudoku
        public bool Solve(int[,] grid)
        {
            for (int row = 1; row < 10; row++)
            {
                Generate_Line();
                for (int col = 1; col < 10; col++)
                {
                    if (grid[row, col] == 0)
                    {
                        for (int num = 1; num < 10; num++)
                        {
                            if (IsValid(grid, row, col, rnd_row[num]))
                            {
                                grid[row, col] = rnd_row[num];

                                if (Solve(grid))
                                {
                                    return true;
                                }
                                else
                                {
                                    grid[row, col] = 0;
                                }
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        public int Count_Solutions(int[,] grid)
        {
            int cnt = 0;
            for (int row = 1; row < 10; row++)
            {
                for (int col = 1; col < 10; col++)
                {
                    if (grid[row, col] == 0)
                    {
                        for (int num = 1; num < 10; num++)
                        {
                            if (IsValid(grid, row, col, num))
                            {
                                grid[row, col] = num;
                                cnt += Count_Solutions(grid);
                                grid[row, col] = 0;
                            }
                        }
                        return cnt;
                    }
                }
            }
            return 1;
        }
    }
    public class Themes
    {
        public Color background_default { get; set; }
        public Color cell_background_default { get; set; }
        public Color cell_background_active { get; set; }
        public Color cell_background_selected { get; set; }
        public Color forecolor_default { get; set; }
        public Color forecolor_wrong { get; set; }
        public Color button_background_default { get; set; }
        private void Blue() //done
        {
            background_default = System.Drawing.ColorTranslator.FromHtml("#379dfc");
            //background_default = Color.LightBlue;
            cell_background_default = System.Drawing.ColorTranslator.FromHtml("#0287fc");
            cell_background_active = System.Drawing.ColorTranslator.FromHtml("#379dfc");
            cell_background_selected = System.Drawing.ColorTranslator.FromHtml("#25d5fc");
            forecolor_default = System.Drawing.ColorTranslator.FromHtml("#003060");
            forecolor_wrong = Color.OrangeRed;
            button_background_default = System.Drawing.ColorTranslator.FromHtml("#25d5fc");
        }
        private void Pink() //done
        {
            background_default = System.Drawing.ColorTranslator.FromHtml("#fc7eef");
            cell_background_default = System.Drawing.ColorTranslator.FromHtml("#f74ace");
            cell_background_active = System.Drawing.ColorTranslator.FromHtml("#fc7eef");
            cell_background_selected = System.Drawing.ColorTranslator.FromHtml("#f754e9");
            forecolor_default = System.Drawing.ColorTranslator.FromHtml("#510e77");
            forecolor_wrong = System.Drawing.ColorTranslator.FromHtml("#c6fc05");
            button_background_default = System.Drawing.ColorTranslator.FromHtml("#f754e9") ;
        }
        private void Green() //done
        {
            //#75E6DA seafoam green
            background_default = System.Drawing.ColorTranslator.FromHtml("#90ee90");
            cell_background_default = System.Drawing.ColorTranslator.FromHtml("#27af18");
            cell_background_active = Color.LightGreen;
            cell_background_selected = Color.LightCyan;
            forecolor_default = Color.Black;
            forecolor_wrong = Color.OrangeRed;
            button_background_default = System.Drawing.ColorTranslator.FromHtml("#02fc49");
        }
        private void Yellow() //done
        {
            background_default = Color.Yellow;
            cell_background_default = Color.Gold;
            cell_background_active = Color.Yellow;
            cell_background_selected = Color.LightYellow;
            forecolor_default = Color.Brown;
            forecolor_wrong = Color.OrangeRed;
            button_background_default = Color.Gold;
        }
        private void Orange() //done
        {
            background_default = Color.FromArgb(252, 157, 55);
            cell_background_default = Color.FromArgb(249, 117, 2);
            cell_background_active = Color.FromArgb(252, 157, 55);
            cell_background_selected = Color.LightYellow;
            forecolor_default = Color.Black;
            forecolor_wrong = Color.Brown;
            button_background_default = Color.FromArgb(249, 117, 2);
        }
        private void Purple() //done
        {
            background_default = Color.FromArgb(145, 95, 255);
            cell_background_default = System.Drawing.ColorTranslator.FromHtml("#661df7"); 
            cell_background_active = Color.FromArgb(145, 95, 255);
            cell_background_selected = Color.FromArgb(124, 65, 252);
            forecolor_default = Color.White;
            forecolor_wrong = Color.Red;
            button_background_default = Color.FromArgb(117, 55, 252);
        }
        private void Dark() //done
        {
            background_default = Color.FromArgb(64, 64, 64);
            cell_background_default = Color.FromArgb(96, 96, 96);
            cell_background_active = Color.FromArgb(64, 64, 64);
            cell_background_selected = Color.FromArgb(128, 128, 128);
            forecolor_default = Color.White;
            forecolor_wrong = Color.OrangeRed;
            button_background_default = Color.FromArgb(96, 96, 96);
        }
        private void Light() //done
        {
            background_default = Color.White;
            cell_background_default = Color.LightGray;
            cell_background_active = Color.White;
            cell_background_selected = Color.LightCyan;
            forecolor_default = Color.Black;
            forecolor_wrong = Color.OrangeRed;
            button_background_default = Color.LightGray;
        }

        //set theme
        public void Set_Theme(string theme)
        {
            switch (theme)
            {
                case "Albastru":
                    Blue();
                    break;
                case "Roz":
                    Pink();
                    break;
                case "Verde":
                    Green();
                    break;
                case "Galben":
                    Yellow();
                    break;
                case "Portocaliu":
                    Orange();
                    break;
                case "Violet":
                    Purple();
                    break;
                case "Intunecat":
                    Dark();
                    break;
                case "Luminos":
                    Light();
                    break;
            }
        }
    }
}