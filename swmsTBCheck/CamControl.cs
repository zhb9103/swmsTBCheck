using System;
using System.Collections.Generic;
using System.Linq;
//using AForge.Video.DirectShow;




namespace swmsTBCheck
{
    /// <summary>
    /// Class to control the cameras of the system.
    /// </summary>
    class CamControl
    {
        //    private int selectedFrameSize;
        //    private int selectedFps;
        //    private VideoCaptureDevice selectedDevice;
        //    public FilterInfoCollection Devices { get; private set; }
        //    public Dictionary<int, string> FrameSizes { get; private set; }
        //    public List<int> Fps;

        //    /// <summary>
        //    /// Gets the selected device.
        //    /// </summary>
        //    public VideoCaptureDevice SelectedDevice
        //    {
        //        get
        //        {
        //            return selectedDevice;
        //        }
        //        private set
        //        {
        //            selectedDevice = value;
        //            RefreshFrameSize();
        //        }
        //    }

        //    /// <summary>
        //    /// Initializes a new instance of the <see cref="CamControl"/> class.
        //    /// </summary>
        //    public CamControl()
        //    {
        //        Devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

        //        // by default select the first one
        //        SetCamera(0);
        //    }

        //    /// <summary>
        //    /// Sets the active camera.
        //    /// </summary>
        //    /// <param name="index">The index of the camera</param>
        //    public void SetCamera(int index)
        //    {
        //        if (Devices.Count < index)
        //        {
        //            throw new IndexOutOfRangeException("There is no device with index " + index);
        //        }

        //        SelectedDevice = new VideoCaptureDevice(Devices[index].MonikerString);
        //    }

        //    /// <summary>
        //    /// Sets the size of the frame.
        //    /// </summary>
        //    /// <param name="index">The index of the available fps</param>
        //    public void SetFrameSize(int index)
        //    {
        //        if (FrameSizes.Count < index)
        //        {
        //            throw new IndexOutOfRangeException("There is no framesize with index " + index);
        //        }

        //        selectedFrameSize = index;
        //        RefreshFps();
        //        ConfigureCamera();
        //    }

        //    /// <summary>
        //    /// Sets the FPS of the active camera.
        //    /// </summary>
        //    /// <param name="fps">The FPS</param>
        //    public void SetFps(int fps)
        //    {
        //        if (!Fps.Contains(fps))
        //        {
        //            throw new IndexOutOfRangeException("There is no fps like " + fps);
        //        }

        //        selectedFps = fps;
        //        ConfigureCamera();
        //    }

        //    /// <summary>
        //    /// Refreshes the size of the frame.
        //    /// </summary>
        //    private void RefreshFrameSize()
        //    {
        //        this.FrameSizes = new Dictionary<int, string>();
        //        int i = 0;
        //        foreach (VideoCapabilities set in SelectedDevice.VideoCapabilities)
        //        {
        //            this.FrameSizes.Add(i, String.Format("{0} x {1}", set.FrameSize.Width, set.FrameSize.Height));
        //            i++;
        //        }

        //        selectedFrameSize = i-1;
        //        try
        //        {
        //            RefreshFps();
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.Message.ToString();
        //            //System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
        //        }
        //    }

        //    /// <summary>
        //    /// Refreshes the FPS.
        //    /// </summary>
        //    private void RefreshFps()
        //    {
        //        int MaxFramerate = selectedDevice.VideoCapabilities[selectedFrameSize].FrameRate;
        //        Fps = new List<int>();
        //        for (int i = 1; i < MaxFramerate; i++)
        //        {
        //            if (i % 5 == 0)
        //            {
        //                Fps.Add(i);
        //            }
        //        }

        //        selectedFps = Fps.Min();
        //    }

        //    /// <summary>
        //    /// Configures the camera.
        //    /// </summary>
        //    private void ConfigureCamera()
        //    {
        //        SelectedDevice.DesiredFrameSize = SelectedDevice.VideoCapabilities[selectedFrameSize].FrameSize;
        //        SelectedDevice.DesiredFrameRate = selectedFps;
        //    }
    }
}
