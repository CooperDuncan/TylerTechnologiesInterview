import pygame
import os

WIDTH, HEIGHT = 1100, 800

TAB_WIDTH = 300
TAB_HEIGHT = 800

ROWS, COLS = 8, 8
SQUARE_SIZE = 800//COLS

FPS = 60

#image
QUEEN_IMG = pygame.image.load(
    os.path.join('queens', 'queen_img.png')
)
QUEEN = pygame.transform.scale(QUEEN_IMG, (SQUARE_SIZE, SQUARE_SIZE))

WHITE_QUEEN_IMG = pygame.image.load(
    os.path.join('queens', 'queen_white_img.png')
)
WHITE_QUEEN = pygame.transform.scale(WHITE_QUEEN_IMG, (SQUARE_SIZE, SQUARE_SIZE))
#rgb
WHITE = (255, 255, 255)
BLACK = (0,0,0)
GRAY = (140,140,140)
DARK_GRAY = (50,50,50)