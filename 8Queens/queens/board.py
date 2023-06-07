from sys import _xoptions
import pygame
from .constants import BLACK, WHITE, GRAY, DARK_GRAY, ROWS, SQUARE_SIZE, TAB_WIDTH, TAB_HEIGHT, QUEEN, WHITE_QUEEN

WIN = pygame.display.set_mode((900, 800))


class Board:
    def __init__(self):
        self.board = []

    def draw_data(self, win, numQueens, solutions):
        count = numQueens
        curr_sol = solutions
        font = pygame.font.SysFont('calibri', 17)
        count_text = font.render("Number of Solutions remaining: " + str(numQueens), 1, (0,0,0))
        win.blit(count_text, (830, 80))

        #list all solutions below
        y_offset = 15
        for s in solutions:
            font = pygame.font.SysFont('calibri',25)
            text = font.render(str(s), 1, (0,0,0))
            win.blit(text, (850, 80 + y_offset))
            y_offset += 20

    def draw_squares(self, win):
        WIN = win
        win.fill(BLACK)
        pygame.draw.rect(win, GRAY, (800, 0, TAB_WIDTH, TAB_HEIGHT))
        pygame.draw.rect(win, DARK_GRAY, (800, 0, 10, TAB_HEIGHT))
        for row in range(ROWS):
            for col in range(row % 2, ROWS, 2):
                pygame.draw.rect(win, WHITE, (row*SQUARE_SIZE, col*SQUARE_SIZE, SQUARE_SIZE, SQUARE_SIZE))

    def draw_queens(self, solution, win):
        WIN = win
        counter = 0
        sol = []
        sol = solution
        queen = QUEEN
        #to draw current_solution above button
        font = pygame.font.SysFont('calibri',25)
        text = font.render(str(sol), 1, (0,0,0))
        win.blit(text, (850, 20))

        for s in sol:
            x = s
            xpos = (x * SQUARE_SIZE)
            ypos = (counter * SQUARE_SIZE)
            if(counter % 2 == 0):
                if (x % 2 == 0):
                    queen = QUEEN
                else:
                    queen = WHITE_QUEEN
            else:
                if (x % 2 == 0):
                    queen = WHITE_QUEEN
                else:
                    queen = QUEEN

            win.blit(queen, (xpos, ypos))
               
            counter+=1  

