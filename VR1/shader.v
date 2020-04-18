#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in float aColor;

out vec4 vertexColor;

void main() 
{
	vec3 newPosition = aPosition + vec3(0.3, 0, 0);
	newPosition = newPosition/2;
	gl_Position = vec4(newPosition, 1.0);	
	vertexColor = vec4(aColor, 0.5, 0.5, 1.0);
}
