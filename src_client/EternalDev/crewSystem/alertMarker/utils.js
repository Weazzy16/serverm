export const drawSprite = (
    textureDict, 
    textureName, 
    { x, y, z }, 
    [ width, height ], 
    heading, 
    colour, 
    offset = { x: 0, y: 0 }
) => {
    mp.game.graphics.setDrawOrigin(x, y, z, 0);
    
    let resolution = mp.game.graphics.getScreenActiveResolution(0, 0),
        textureResolution = mp.game.graphics.getTextureResolution(textureDict, textureName),
        textureScale = [width * textureResolution.x / resolution.x, height * textureResolution.y / resolution.y];

    if (mp.game.graphics.hasStreamedTextureDictLoaded(textureDict)) {                
        mp.game.graphics.drawSprite(
            textureDict, 
            textureName, 
            offset.x, 
            offset.y, 
            textureScale[0], 
            textureScale[1],
            heading, 
            colour.r, 
            colour.g, 
            colour.b, 
            colour.a,
            true
        );
    } 
    else 
        mp.game.graphics.requestStreamedTextureDict(textureDict, true);
    
    mp.game.graphics.clearDrawOrigin();
}