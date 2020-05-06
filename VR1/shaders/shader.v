#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;


uniform float scale;


out vec4 vertexColor;
out vec2 center;

void main() {
	vec3 newPosition = aPosition * scale;
	gl_Position = vec4(newPosition, 1.0);
	vertexColor = vec4(aColor, 1.0);
}