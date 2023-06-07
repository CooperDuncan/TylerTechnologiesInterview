
import collections
import time
import pygame
from pstats import SortKey
from queens.board import Board

numQueens = 8

current_solution = [0 for x in range(numQueens)] #holds current placements for queens
solutions = [] #store found solutions   


def isSafe(currRow, col):
    #every space is safe in row0
    if currRow == 0:
        return True
    
    for row in range(0, currRow):
        #check vertical spaces
        if col == current_solution[row]:
            return False

        #diagonals
        if abs(currRow - row) == abs(col - current_solution[row]):
            return False

    #spot is safe
    return True

def findQueens(row):
    global current_solution, solutions, numQueens

    for col in range(numQueens):
        if not isSafe(row, col):
            continue
        else:
            current_solution[row] = col
            #if in last row
            if row == (numQueens - 1):
                solutions.append(current_solution.copy())
            else:
                findQueens(row + 1)
class Queen():

    def startSearch(void):
        print("Solving for " + str(numQueens) + " Queens")
        time.sleep(2)
        findQueens(0)

        print(len(solutions), " solutions found")
        for solution in solutions:
            print(solution)
        return solutions.pop(0)

    def return_queen(void):
        return solutions.pop(0)
    
    def  return_data(self, win):
        board = Board()
        board.draw_data(win, len(solutions), solutions)
    
        

