using Veldrid;

namespace WinterEngine.Rendering;
public class TextureHandle {
    private static readonly ILog log = LogManager.GetLogger(typeof(TextureHandle));

    public Texture Texture => _surfaceTexture;
    public TextureView TextureView => _surfaceTextureView;
    
    private Texture _surfaceTexture;
    private TextureView _surfaceTextureView;

    public TextureHandle(Texture surfaceTexture) {
        _surfaceTexture = surfaceTexture;

        _surfaceTextureView = Renderer.GraphicsDevice.ResourceFactory.CreateTextureView(_surfaceTexture);
    }

    ~TextureHandle() {
        if (!_surfaceTexture.IsDisposed || !_surfaceTextureView.IsDisposed) {
            // don't run it ourselves, encourage developers to manually dispose for best practices
            // that way we always know when things are getting disposed of.
            log.Warn("Resources Leaked! Make sure you're calling Dispose before deconstruction!");
        }
    }
    
    public void Dispose() {
        _surfaceTexture.Dispose();
        _surfaceTextureView.Dispose();
    }
}
