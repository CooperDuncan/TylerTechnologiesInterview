import pygame
import os
import cProfile
import pstats 
from pstats import SortKey
from queens.constants import WIDTH, HEIGHT, SQUARE_SIZE, FPS, QUEEN
from queens.board import Board
from queens.EightQueens import Queen
from queens.button import Button


WIN = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("8 Queens")

current_solution = []
sol = []
queen = Queen()

def main():
    run = True
    clock = pygame.time.Clock()
    board = Board()
    button = Button((255,255,255), 875, 50, 150, 25, 'Next Set')

    #cProfile.run('queen.startSearch()', "output.dat")

    #with open("output_time.txt", "w") as f:
    #    p = pstats.Stats("output.dat", stream = f)
    #    p.sort_stats("time").print_stats()

    #with open("output_calls.txt", "w") as f:
    #    p = pstats.Stats("output.dat", stream = f)
    #    p.sort_stats("calls").print_stats()
        
    sol = queen.startSearch()
    while run:
        clock.tick(FPS)
        pygame.display.update()
        for event in pygame.event.get():
            pos = pygame.mouse.get_pos()
            if event.type == pygame.QUIT:
                run = False
            if event.type == pygame.MOUSEBUTTONDOWN:
                if button.isOver(pos):
                    sol = queen.return_queen()

        board.draw_squares(WIN)
        board.draw_queens(sol, WIN)
        queen.return_data(WIN)
        button.draw(WIN, True)

    pygame.quit() 


    
main()