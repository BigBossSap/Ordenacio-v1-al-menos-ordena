using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ordenacio.Model;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Ordenacio.Enums.Enums;

namespace Ordenacio.ViewModel
{
    internal partial class PrincipalViewModel : ObservableObject
    {
        #region Propietats

        [ObservableProperty]
        private ObservableCollection<PositionedRectangle> rectangulos;

        [ObservableProperty]
        private int[] numeros;

        [ObservableProperty]
        private Brush colorPinzell = Brushes.Black;

        [ObservableProperty]
        public Color colorCorrecte;

        [ObservableProperty]
        private Color colorIncorrecte;

        [ObservableProperty]
        private Color colorIntercanvi;

        [ObservableProperty]
        private int quantitatNumeros;

        [ObservableProperty]
        private string figuraSeleccionada;

        [ObservableProperty]
        private string[] llistaFigures;

        [ObservableProperty]
        private string tipusIntercanviSeleccionat;

        [ObservableProperty]
        private string[] llistaTipusIntercanvi;

        [ObservableProperty]
        private ImageBrush stroke;

        [ObservableProperty]
        private bool isElipse;

        [ObservableProperty]
        private int zIndex;

        [ObservableProperty]
        private ImageBrush fill;

        [ObservableProperty]
        private int radiColumnes;

        [ObservableProperty]
        private int tempsPausa;

        [ObservableProperty]
        private double radiusX;

        [ObservableProperty]
        private double radiusY;

        [ObservableProperty]
        private double x;

        [ObservableProperty]
        private double y;

        private Action EmptyDelegate = delegate () { };

        private int numRectangle;

        #endregion Propietats

        [RelayCommand]
        private void Generar()
        {
            Numeros = Enumerable.Range(1, quantitatNumeros).ToArray();

            Random rand = new Random();

            for (int i = 0; i < Numeros.Length; i++)
            {
                int randomIndex = rand.Next(i, Numeros.Length);

                int temp = Numeros[i];
                Numeros[i] = Numeros[randomIndex];
                Numeros[randomIndex] = temp;
            }

            GenerarRectangles();
        }

        public async Task OrdenarRectangulosAsync()
        {
            double espacioTotal = 50;  // Reduced total space between rectangles
            double x = 0;
            double espacio = espacioTotal / (Numeros.Count() - 1);  // Adjusted space between rectangles
            double anchoTotal = 1000 - espacioTotal;
            double ancho = anchoTotal / Numeros.Count();
            for (int i = 0; i < Numeros.Count() - 1; i++)
            {
                for (int j = 0; j < Numeros.Count() - i - 1; j++)
                {
                    if (Numeros[j] > Numeros[j + 1])
                    {
                        // Intercambia los números
                        int temp = Numeros[j];
                        Numeros[j] = Numeros[j + 1];
                        Numeros[j + 1] = temp;

                        // Intercambia los rectángulos
                        PositionedRectangle tempRect = Rectangulos[j];
                        Rectangulos[j] = Rectangulos[j + 1];
                        Rectangulos[j + 1] = tempRect;

                        await SwapAndAnimateAsync(Rectangulos[j], Rectangulos[j + 1]);

                        DoEvents();
                        await Task.Delay(500);
                    }
                }
            }

            // After sorting, update the X property of each rectangle in the Rectangulos ObservableCollection to reflect its new position.
            x = 0;
            foreach (var rect in Rectangulos)
            {
                rect.X = x;
                x += rect.Rectangle.Width + espacio;
            }
        }

        public void GenerarRectangles()
        {
            Rectangulos = new ObservableCollection<PositionedRectangle>();
            double espacioTotal = 50;  // Reduced total space between rectangles
            double x = 0;
            double espacio = espacioTotal / (Numeros.Count() - 1);  // Adjusted space between rectangles
            double anchoTotal = 1000 - espacioTotal;
            double ancho = anchoTotal / Numeros.Count();

            int maxNumero = Numeros.Max();

            foreach (int numero in Numeros)
            {
                double alturaNormalizada = (numero / (double)maxNumero) * 1000;

                Rectangle rect = new Rectangle
                {
                    Width = ancho,
                    Height = alturaNormalizada,
                    Fill = Brushes.Blue,
                };

                PositionedRectangle rectangulo = new PositionedRectangle
                {
                    Rectangle = rect,
                    X = x + espacio,
                    Y = 1000 - alturaNormalizada,
                    IsElipse = IsElipse,
                    Number = numero,
                };
                Rectangulos.Add(rectangulo);

                x += rect.Width + espacio;
            }
        }

        public async Task SwapAndAnimateAsync(PositionedRectangle rect1, PositionedRectangle rect2)
        {
            // Get the initial positions
            double x1 = rect1.X;
            double x2 = rect2.X;

            // Reset the TranslateTransform of each rectangle
            rect1.Rectangle.RenderTransform = new TranslateTransform();
            rect2.Rectangle.RenderTransform = new TranslateTransform();

            // Create animations
            DoubleAnimation animation1 = new DoubleAnimation(x2 - x1, TimeSpan.FromSeconds(0.5));
            DoubleAnimation animation2 = new DoubleAnimation(x1 - x2, TimeSpan.FromSeconds(0.5));

            // Create the storyboard
            Storyboard storyboard = new Storyboard();

            // Set the target of the animations to the rectangles
            Storyboard.SetTarget(animation1, rect1.Rectangle);
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            Storyboard.SetTarget(animation2, rect2.Rectangle);
            Storyboard.SetTargetProperty(animation2, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

            // Add the animations to the storyboard
            storyboard.Children.Add(animation1);
            storyboard.Children.Add(animation2);

            // Begin the storyboard
            storyboard.Begin();

            // Wait for the animation to complete before continuing with the next swap
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            // Update the positions in the data structure after the animation
            rect1.X = x2;
            rect2.X = x1;
        }

        public PrincipalViewModel()
        {
            llistaFigures = Enum.GetNames(typeof(tipusFigura));
            llistaTipusIntercanvi = Enum.GetNames(typeof(SortMethod));
            colorCorrecte = Colors.Violet;
            colorIncorrecte = Colors.Red;
            colorIntercanvi = Colors.Green;
            figuraSeleccionada = llistaFigures[0];
            tipusIntercanviSeleccionat = llistaTipusIntercanvi[0];
            isElipse = true;
            tempsPausa = 2;
            quantitatNumeros = 10;
        }

        [RelayCommand]
        public void Ordenar()
        {
            //LoadRectangles(Numeros);
            //SortArray();
            //SortRectangles();

            OrdenarRectangulosAsync();
            DoEvents();
        }

        public void SortRectangles()
        {
        }

        public void SortArray()
        {
            if (tipusIntercanviSeleccionat == Ordenacio.Enums.Enums.SortMethod.BubbleSort.ToString())
            {
                BubbleSort(Numeros);
            }
            else if (tipusIntercanviSeleccionat == Ordenacio.Enums.Enums.SortMethod.InsertionSort.ToString())
            {
                InsertionSort(Numeros);
            }
            else if (tipusIntercanviSeleccionat == Ordenacio.Enums.Enums.SortMethod.SelectionSort.ToString())
            {
                SelectionSort(Numeros);
            }
        }

        #region Threads

        private Thread thread;

        private void Espera(double milliseconds)
        {
            var frame = new DispatcherFrame();
            thread = new Thread((ThreadStart)(() =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(milliseconds));
                frame.Continue = false;
            }));
            thread.Start();
            Dispatcher.PushFrame(frame);
        }

        private static Action action;

        public static void DoEvents()
        {
            action = new Action(delegate { });
            Application.Current?.Dispatcher?.Invoke(
               System.Windows.Threading.DispatcherPriority.Background,
               action);
        }

        public static void BubbleSort(int[] array)
        {
            int temp = 0;

            for (int j = 0; j <= array.Length - 2; j++)
            {
                for (int i = 0; i <= array.Length - 2; i++)
                {
                    if (array[i] > array[i + 1])
                    {
                        // Intercambia los elementos en el array
                        temp = array[i + 1];
                        array[i + 1] = array[i];
                        array[i] = temp;
                    }
                }
            }
        }

        public static void InsertionSort(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int key = array[i];
                int j = i - 1;
                while (j >= 0 && array[j] > key)
                {
                    array[j + 1] = array[j];
                    j = j - 1;
                }
                array[j + 1] = key;
            }
        }

        public static void SelectionSort(int[] array)
        {
            int n = array.Length;

            for (int i = 0; i < n - 1; i++)
            {
                int min_idx = i;
                for (int j = i + 1; j < n; j++)
                    if (array[j] < array[min_idx])
                        min_idx = j;

                int temp = array[min_idx];
                array[min_idx] = array[i];
                array[i] = temp;
            }
        }
    }

    #endregion Threads
}