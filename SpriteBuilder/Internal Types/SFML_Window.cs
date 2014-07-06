using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;
using KeyEventArgs = SFML.Window.KeyEventArgs;

namespace SpriteBuilder
{
    public class SFML_Window
    {
        private double _timePerFrame = 1 / 60f;
        private RenderWindow _window;
        private Stopwatch stopwatch;
        private Font _statsFont;
        private Text _text;
        private double _statisticsUpdateTime;
        private double _statisticsNumFrames;
        private SpriteBuilderForm main;
        private List<Sprite> _spriteList;
        private Sprite _currentSprite;
        private int _currentFrame;
        private double _elapsedTime;
        private bool _repeat;

        private object _lockObject = new object();


        public SFML_Window(SpriteBuilderForm form, DrawingSurface surface)
        {
            main = form;
            _window = new RenderWindow(surface.Handle);
            _window.Closed += OnClose;
            _window.KeyPressed += OnKeyPressed;
            _window.SetFramerateLimit(60);

            _spriteList = new List<Sprite>();
            _currentFrame = 0;
            _elapsedTime = 0;
            _repeat = true;

            SpriteBuilderForm.StagedImageList.ListChanged += ListChanged;

            Init();
        }


        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                _window.Close();
        }


        private void Init()
        {
            try
            {
                // http://www.dafont.com/sansation.font
                _statsFont = new Font("Content/Font/Sansation.ttf");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to load font: " + e);
                _window.Close();
            }

            _text = new Text("Loading stats...", _statsFont)
            {
                Position = new Vector2f(5f, 5f),
                CharacterSize = 14
            };
        }

        public void Run()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeSinceLastUpdate = 0;
            double elapsedTime;

            while (main.Visible) // Main loop
            {
                elapsedTime = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                timeSinceLastUpdate += elapsedTime;

                while (timeSinceLastUpdate > _timePerFrame)
                {
                    timeSinceLastUpdate -= _timePerFrame;

                    // process events
                    System.Windows.Forms.Application.DoEvents(); // Form Events
                    _window.DispatchEvents(); // SFML Events
                    Update(_timePerFrame);
                }

                UpdateStatistics(elapsedTime);
                Render();

            }
        }

        private void Update(double delta)
        {
            // CheckForNewImages();

            if (_spriteList.Count > 0 && SpriteBuilderForm.Play )
            {
                // Animation
                Double timePerFrame = SpriteBuilderForm.Duration / (float)_spriteList.Count;
                _elapsedTime += delta;

                if (_currentFrame == 0)
                    _currentSprite = _spriteList[0];

                while (_elapsedTime >= timePerFrame && (_currentFrame <= _spriteList.Count || _repeat))
                {
                    // next
                    _elapsedTime -= timePerFrame;
                    if (_repeat)
                    {
                        _currentFrame = (_currentFrame + 1) % _spriteList.Count;

                        if (_currentFrame == 0)
                            _currentSprite = _spriteList[0];
                    }
                    else
                    {
                        _currentFrame++;
                        if (_currentFrame >= _spriteList.Count)
                            _currentFrame = 0;
                    }

                    _currentSprite = _spriteList[_currentFrame];
                }
            }
        }

        private void ListChanged(object sender, ListChangedEventArgs e)
        {
            _spriteList.Clear();

            lock (main) // The form is threaded need to be careful here accessing the window component
            {
                foreach (var texture in from item in SpriteBuilderForm.StagedImageList select item.FilePath into path where path != null select new Texture(path))
                {
                    var sprite = new Sprite(texture);
                    var scaleFactor = (250/sprite.GetLocalBounds().Width)*0.75f;
                    sprite.Scale = new Vector2f(scaleFactor,  scaleFactor);
                    sprite.Position = new Vector2f(250 - (sprite.GetLocalBounds().Width / 2), 250 - (sprite.GetLocalBounds().Height / 2));
                    _spriteList.Add(sprite);
                }
            }
        }

        private void Render()
        {
            _window.Clear(new Color(155, 187, 252));
            // Draw calls
            if (_currentSprite != null)
                _window.Draw(_currentSprite);
            _window.Draw(_text);
            _window.Display();
        }

        private void AddToSpriteList(string path)
        {

        }

        private void UpdateStatistics(double elapsedTime)
        {
            _statisticsUpdateTime += elapsedTime;
            _statisticsNumFrames += 1;

            if (_statisticsUpdateTime >= 1)
            {
                _text.DisplayedString = "Frames / Second = " + _statisticsNumFrames + "\n" +
                                       "Time / Update = " + string.Format("{0:0}", (_statisticsUpdateTime * 1000000) / _statisticsNumFrames) + "us";

                _statisticsUpdateTime -= 1;
                _statisticsNumFrames = 0;
            }

        }

        private void OnClose(object sender, EventArgs e)
        {
            var window = (RenderWindow)sender;
            window.Close();
        }

    }
}
