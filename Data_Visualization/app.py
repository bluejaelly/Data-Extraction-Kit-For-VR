import pandas as pd
import math
import numpy as np
import pathlib
from pathlib import Path
import dash
import datetime as dt
from datetime import datetime
from dash.dependencies import Input, Output, State, ClientsideFunction, MATCH, ALL, ALLSMALLER
import dash_core_components as dcc
import dash_html_components as html
import dash_table
import plotly.express as px
import plotly.graph_objects as go
from plotly.subplots import make_subplots
import glob
import os
import json
import io

app = dash.Dash(
    name = __name__,
    assets_folder = "assets",
    meta_tags= [{"name": "viewport", "content": "width=device-width"}]
)

server = app.server

# Import temp datasets
head_data, point_data, grab_data, pos_data = importAllDatasets(os.getcwd() + '/*')

####################
# helper functions #
####################


def findIndex(ref_list, keyword):
    try:
        return [idx  for idx, s in enumerate(ref_list) if keyword in s][0]
    except:
        return None


def importAllDatasets(directory):
    """
    A helper function to import all the datasets for the project from the input directory parameter.

    Parameter
    _________
    directory : str
        directory to the datasets

    Returns
    _______
    head_data: Pandas Dataframe
        A dataframe of a head gaze data

    point_data: Pandas Dataframe
        A dataframe of a controller point data

    grab_data: Pandas Dataframe
        A dataframe of a controller grab data

    pos_data: Pandas Dataframe
        A dataframe of a player position data
    
    """
    head_index = findIndex(temp_list, "Gaze")
    point_index = findIndex(temp_list, "Point")
    grab_index = findIndex(temp_list, "Grab")
    pos_index = findIndex(temp_list, "Position")

    head_data = pd.read_csv(temp_list[head_index]) if head_index != None else None
    point_data = pd.read_csv(temp_list[point_index]) if point_index != None else None
    grab_data = pd.read_csv(temp_list[grab_index]) if grab_index != None else None
    pos_data = pd.read_csv(temp_list[pos_index]) if pos_index != None else None


    return head_data, point_data, grab_data, pos_data


app.layout = html.Div(
    [
        dcc.Store(id="aggregate_data"),
        html.Div(id="output-clientside"),

        # Header
        html.Div(
            [
                html.Div(
                    html.H3(
                        "Data Extraction Kit for VR",
                        style={"marginBottom": "25px", "textAlign": "center"}
                    ),
                    id="header",
                    className = "row flex_display",
                    style={"marginBottom": "25px"}
                ),

                # Tabs(project selection)
                html.Div(
                    [
                        dcc.Tabs(id="tabs", className="custom-tabs", children=[
                            dcc.Tab(label='Head Gaze', value='gaze', className="custom-tab", style={"backgroundColor":"#e6e6e6"}, children=[
                                html.Div(
                                    [
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        ),
                                        html.Div(
                                            [
                                                dcc.Graph()
                                            ],
                                            className = "row flex_display",
                                        )
                                    ]
                                )  
                            ]),
                            dcc.Tab(label='Controller Point', value='point', className="custom-tab", style={"backgroundColor":"#e6e6e6"},children=[
                                html.Div(
                                    [
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        ),
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        )
                                    ]
                                )  
                            ]),
                            dcc.Tab(label='Controller Grab', value='grab', className="custom-tab", style={"backgroundColor":"#e6e6e6"},children=[
                                html.Div(
                                    [
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        ),
                                        html.Div(
                                            [
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph()
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        )
                                    ]
                                )  
                            ]),
                            dcc.Tab(label='Player Position', value='position', className="custom-tab", style={"backgroundColor":"#e6e6e6"}, children=[
                                dcc.Graph(id="position-heatmap", responsive=True, config={'displaylogo': False, 'responsive':True}, style={"height" : "100%", "width" : "100%"})
                            ]),
                        ]),
                        html.Div(id='cur-tab', style={'display':'none'})
                    ],
                    id="tabs-container",
                    className="row flex_display",
                ),
            ]
        )
    ]
)



if __name__ == "__main__":
    app.run_server(debug = True)