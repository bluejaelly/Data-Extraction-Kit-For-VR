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

# Graph Modules
from data_vis import VisGraphs




app = dash.Dash(
    name = __name__,
    assets_folder = "assets",
    meta_tags= [{"name": "viewport", "content": "width=device-width"}]
)

server = app.server

DATA_DIRECTORY = os.getcwd() + '/data'
DEFAULT_GRAPH = {'data': [], 'layout': {}}

####################
# helper functions #
####################


def findIndex(ref_list, keyword):
    try:
        return [idx  for idx, s in enumerate(ref_list) if keyword in s][0]
    except:
        return None


def importAllDatasets(directory, userID):

    if not userID: return None, None, None, None
    user_dir = '{}/{}'.format(directory, userID)
    dir_list = glob.glob(user_dir + '/*')
    gaze_dir = user_dir + '/{}_GazeData.csv'.format(userID)
    point_dir = user_dir + '/{}_ControllerPointData.csv'.format(userID)
    grab_dir = user_dir + '/{}_ControllerGrabData.csv'.format(userID)
    pos_dir = user_dir + '/{}_PlayerPositionData.csv'.format(userID)

    gaze_data = pd.read_csv(gaze_dir) if gaze_dir in dir_list else None
    point_data = pd.read_csv(point_dir) if point_dir in dir_list else None
    grab_data = pd.read_csv(grab_dir) if grab_dir in dir_list else None
    pos_data = pd.read_csv(pos_dir) if pos_dir in dir_list else None

    return gaze_data, point_data, grab_data, pos_data


def getUserIDList():
    user_dir_list = glob.glob(DATA_DIRECTORY + '/*')
    user_dir_list = [i.rsplit('/', 1)[-1] for i in user_dir_list]
    return [{'label': s, 'value': s} for s in user_dir_list]


# import all datasets

gaze_data, point_data, grab_data, pos_data = importAllDatasets(DATA_DIRECTORY, None)
user_id_list = getUserIDList()
vis_graph = VisGraphs(None, point_data, grab_data, gaze_data, pos_data)

# l_controller_point_avg, r_controller_point_avg, l_controller_point_total, r_controller_point_total, l_controller_point_box, r_controller_point_box = DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH
# both_controller_point_avg, both_controller_point_total, both_controller_point_box = DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH

# l_grab_avg, r_grab_avg, l_grab_total, r_grab_total, l_grab_box, r_grab_box  = DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH

# b_grab_avg, b_grab_total, b_grab_box = DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH

# gaze_avg, gaze_total, gaze_box = DEFAULT_GRAPH, DEFAULT_GRAPH, DEFAULT_GRAPH


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

                html.Div(
                    [                        
                        dcc.Dropdown(
                            id = "user_id",
                            multi = False,
                            className="dcc_control",
                            style={"marginBottom": "25px", "marginLeft": "5px", "marginRight": "20px"},
                            options=user_id_list
                        ),
                        html.Div(
                            id = "cur_user_id",
                            style={"display":"none"}
                        )
                    ],
                    className = "row flex_display",
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
                                                        dcc.Graph(id="gaze_avg")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="gaze_total")
                                                    ],
                                                    className="one-half column"
                                                ),
                                            ],
                                            className = "row flex_display",
                                        ),
                                        html.Div(
                                            [
                                                dcc.Graph(id="gaze_box")
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
                                                        dcc.Graph(id="l_controller_point_avg")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="r_controller_point_avg")
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
                                                        dcc.Graph(id="l_controller_point_total")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="r_controller_point_total")
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
                                                        dcc.Graph(id="l_controller_point_box")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="r_controller_point_box")
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
                                                        dcc.Graph(id="both_controller_point_avg")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="both_controller_point_total")
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
                                                        dcc.Graph(id="both_controller_point_box")
                                                    ],
                                                    className="one-half column"
                                                )
                                            ],
                                            className = "row flex_display",
                                        ),
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
                                                        dcc.Graph(id="l_grab_avg")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="r_grab_avg")
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
                                                        dcc.Graph(id="l_grab_total")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="r_grab_total")
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
                                                        dcc.Graph(id="l_grab_box")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="r_grab_box")
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
                                                        dcc.Graph(id="b_grab_avg")
                                                    ],
                                                    className="one-half column"
                                                ),
                                                html.Div(
                                                    [
                                                        dcc.Graph(id="b_grab_total")
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
                                                        dcc.Graph(id="b_grab_box")
                                                    ],
                                                    className="one-half column"
                                                )
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

@app.callback(Output("cur_user_id", "children"), [Input("user_id", "value")])
def updateCurrentUserID(user_id):
    return user_id

@app.callback(
    [
        Output("l_controller_point_avg", "figure"),
        Output("r_controller_point_avg", "figure"),
        Output("l_controller_point_total", "figure"),
        Output("r_controller_point_total", "figure"),
        Output("l_controller_point_box", "figure"),
        Output("r_controller_point_box", "figure"),
        Output("both_controller_point_avg", "figure"),
        Output("both_controller_point_total", "figure"),
        Output("both_controller_point_box", "figure"),
        Output("l_grab_avg", "figure"),
        Output("r_grab_avg", "figure"),
        Output("l_grab_total", "figure"),
        Output("r_grab_total", "figure"),
        Output("l_grab_box", "figure"),
        Output("r_grab_box", "figure"),
        Output("b_grab_avg", "figure"),
        Output("b_grab_total", "figure"),
        Output("b_grab_box", "figure"),
        Output("gaze_avg", "figure"),
        Output("gaze_total", "figure"),
        Output("gaze_box", "figure")        
    ],
    [
        Input("cur_user_id", "children")
    ]
)
def updateGraphs(user_id):
    gaze_data, point_data, grab_data, pos_data = importAllDatasets(DATA_DIRECTORY, user_id)
    vis_graph.update_data(user_id, point_data, grab_data, gaze_data, pos_data)

    l_controller_point_avg = vis_graph.plot_controller_point_avg('LeftControllerPoint')
    r_controller_point_avg = vis_graph.plot_controller_point_avg('RightControllerPoint')
    l_controller_point_total = vis_graph.plot_controller_point_total('LeftControllerPoint')
    r_controller_point_total = vis_graph.plot_controller_point_total('RightControllerPoint')
    l_controller_point_box = vis_graph.plot_box_controller_point('LeftControllerPoint')
    r_controller_point_box = vis_graph.plot_box_controller_point('RightControllerPoint')
    both_controller_point_avg, both_controller_point_total, both_controller_point_box = vis_graph.plot_both_controller_point()

    l_grab_avg = vis_graph.plot_controller_grab_avg('LeftControllerGrab')
    r_grab_avg = vis_graph.plot_controller_grab_avg('RightControllerGrab')
    l_grab_total = vis_graph.plot_controller_grab_total('LeftControllerGrab')
    r_grab_total = vis_graph.plot_controller_grab_total('RightControllerGrab')
    l_grab_box = vis_graph.plot_box_controller_grab('LeftControllerGrab')
    r_grab_box = vis_graph.plot_box_controller_grab('RightControllerGrab')
    b_grab_avg, b_grab_total, b_grab_box = vis_graph.plot_both_controller_grab()

    gaze_avg = vis_graph.plot_gaze_avg()
    gaze_total = vis_graph.plot_gaze_total()
    gaze_box = vis_graph.plot_box_gaze()

    return l_controller_point_avg, r_controller_point_avg, l_controller_point_total, r_controller_point_total, l_controller_point_box, r_controller_point_box, both_controller_point_avg, both_controller_point_total, both_controller_point_box, l_grab_avg, r_grab_avg, l_grab_total, r_grab_total, l_grab_box, r_grab_box, b_grab_avg, b_grab_total, b_grab_box, gaze_avg, gaze_total, gaze_box


if __name__ == "__main__": 
    app.run_server(debug = True)